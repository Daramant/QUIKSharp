// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using QuikSharp.Messages;
using QuikSharp.Json.Serializers;
using QuikSharp.Extensions;
using QuikSharp.Exceptions;
using QuikSharp.QuikEvents;
using QuikSharp.Quik;
using QuikSharp.IdProviders;

namespace QuikSharp.QuikClient
{
    /// <summary>
    ///
    /// </summary>
    public sealed class QuikClient : IQuikClient
    {
        private readonly AsyncManualResetEvent _connectedMre = new AsyncManualResetEvent();

        private readonly IEventInvoker _eventInvoker;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IIdProvider _idProvider;
        private readonly QuikClientOptions _options;

        private IQuik _quik;

        static QuikClient()
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
        }

        public QuikClient(
            IEventInvoker eventInvoker, 
            IJsonSerializer jsonSerializer,
            IIdProvider idProvider,
            QuikClientOptions options)
        {
            _eventInvoker = eventInvoker;
            _jsonSerializer = jsonSerializer;
            _idProvider = idProvider;
            _options = options;
        }

        #region Events

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp успешно подключилась к Quik'у
        /// </summary>
        public event QuikEventHandler<InitEventArgs> Connected;

        private void OnConnected(int port)
        {
            Connected?.Invoke(_quik, new InitEventArgs(port));
        }

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp была отключена от Quik'а
        /// </summary>
        public event QuikEventHandler<EventArgs> Disconnected;

        private void OnDisconnected()
        {
            Disconnected?.Invoke(_quik, EventArgs.Empty);
        }

        #endregion

        public void SetEventSender(IQuik quik)
        {
            _quik = quik;
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsStarted { get; private set; }

        private readonly object _syncRoot = new object();

        private TcpClient _commandClient;
        private TcpClient _eventClient;

        private Task _commandTask;
        private Task _resultTask;
        private Task _eventReceiverTask;
        private Task _eventInvokerTask;

        private readonly Channel<IEvent> _eventChannel =
            Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true
            });

        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;
        private CancellationTokenRegistration _cancellationTokenRegistration;

        /// <summary>
        /// IQuickCalls functions enqueue a message and return a task from TCS
        /// </summary>
        private readonly BlockingCollection<ICommand> _commandQueue = new BlockingCollection<ICommand>();

        /// <summary>
        /// If received message has a correlation id then use its Data to SetResult on TCS and remove the TCS from the dic
        /// </summary>
        private readonly ConcurrentDictionary<long, PendingResult> _pendingResults = new ConcurrentDictionary<long, PendingResult>();

        /// <summary>
        ///
        /// </summary>
        public Task StopAsync()
        {
            if (!IsStarted) 
                return Task.CompletedTask;

            IsStarted = false;
            _cancellationTokenSource.Cancel();
            _cancellationTokenRegistration.Dispose();

            try
            {
                // here all tasks must exit gracefully
                var isCleanExit = Task.WaitAll(new[] {_commandTask, _resultTask, _eventReceiverTask}, 5000);
                Trace.Assert(isCleanExit, "All tasks must finish gracefully after cancellation token is cancelled!");
            }
            finally
            {
                // cancel responses to release waiters
                foreach (var responseKey in _pendingResults.Keys.ToList())
                {
                    if (_pendingResults.TryRemove(responseKey, out var pendingResponse))
                    {
                        pendingResponse.TaskCompletionSource.TrySetCanceled();
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ApplicationException">Response message id does not exists in results dictionary</exception>
        public void Start()
        {
            if (IsStarted) 
                return;

            IsStarted = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _cancellationTokenRegistration = _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetResult(true));

            // Request Task
            _commandTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // Enter the listening loop.
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            Trace.WriteLine("Connecting on request/response channel... ");
                            EnsureConnectedClient(_cancellationTokenSource.Token);
                            // here we have a connected TCP client
                            Trace.WriteLine("Request/response channel connected");
                            try
                            {
                                using (var stream = new NetworkStream(_commandClient.Client))
                                using (var writer = new StreamWriter(stream))
                                {
                                    while (!_cancellationTokenSource.IsCancellationRequested)
                                    {
                                        ICommand command = null;
                                        try
                                        {
                                            // BLOCKING
                                            command = _commandQueue.Take(_cancellationTokenSource.Token);
                                            var serializedCommand = _jsonSerializer.Serialize(command);
                                            //Trace.WriteLine("Request: " + request);
                                            // scenario: Quik is restarted or script is stopped
                                            // then writer must throw and we will add a message back
                                            // then we will iterate over messages and cancel expired ones
                                            if (!command.ValidUntil.HasValue || command.ValidUntil >= DateTime.UtcNow)
                                            {
                                                writer.WriteLine(serializedCommand);
                                                writer.Flush();
                                            }
                                            else
                                            {
                                                Trace.Assert(command.Id > 0, "All requests must have correlation id");
                                                _pendingResults[command.Id]
                                                    .TaskCompletionSource.SetException(
                                                        new TimeoutException("ValidUntilUTC is less than current time"));

                                                _pendingResults.TryRemove(command.Id, out var _);
                                            }
                                        }
                                        catch (OperationCanceledException)
                                        {
                                            // EnvelopeQueue.Take(_cts.Token) was cancelled via the token
                                        }
                                        catch (IOException)
                                        {
                                            // this catch is for unexpected and unchecked connection termination
                                            // add back, there was an error while writing
                                            if (command != null)
                                            {
                                                _commandQueue.Add(command);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                            catch (IOException e)
                            {
                                Trace.TraceError(e.ToString());
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceInformation("Request task is cancelling");
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                        _cancellationTokenSource.Cancel();
                        throw new QuikException("Unhandled exception in background task", e);
                    }
                    finally
                    {
                        lock (_syncRoot)
                        {
                            if (_commandClient != null)
                            {
                                _commandClient.Client.Shutdown(SocketShutdown.Both);
                                _commandClient.Close();
                                _commandClient = null; // У нас два потока работают с одним сокетом, но только один из них должен его закрыть !
                                Trace.WriteLine("Response channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            // Response Task
            _resultTask = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // Поток Response использует тот же сокет, что и поток request
                            EnsureConnectedClient(_cancellationTokenSource.Token);
                            // here we have a connected TCP client

                            try
                            {
                                using (var stream = new NetworkStream(_commandClient.Client))
                                using (var reader = new StreamReader(stream, Encoding.GetEncoding(1251)))
                                {
                                    while (!_cancellationTokenSource.IsCancellationRequested)
                                    {
                                        var readLineTask = reader.ReadLineAsync();
                                        var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);
                                        if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                        {
                                            break;
                                        }

                                        Trace.Assert(readLineTask.Status == TaskStatus.RanToCompletion);
                                        var serializedResult = readLineTask.Result;
                                        if (serializedResult == null)
                                        {
                                            throw new IOException("Lua returned an empty response or closed the connection");
                                        }

                                        // No IO exceptions possible for response, move its processing
                                        // to the threadpool and wait for the next message
                                        // A new task here gives c.30% boost for full TransactionSpec echo

                                        // ReSharper disable once UnusedVariable
                                        var doNotAwaitMe = Task.Factory.StartNew(serializedResultObj =>
                                        {
                                            //var r = response;
                                            //Trace.WriteLine("Response:" + response);
                                            try
                                            {
                                                var response = _jsonSerializer.Deserialize<IResult>((string)serializedResultObj);
                                                Trace.Assert(response.Id > 0);
                                                // it is a response message
                                                if (!_pendingResults.ContainsKey(response.Id))
                                                    throw new QuikException($"Unexpected correlation id: {response.Id}.");

                                                _pendingResults.TryRemove(response.Id, out var pendingResponse);
                                                if (!pendingResponse.Command.ValidUntil.HasValue || pendingResponse.Command.ValidUntil >= DateTime.UtcNow)
                                                {
                                                    pendingResponse.TaskCompletionSource.SetResult(response);
                                                }
                                                else
                                                {
                                                    pendingResponse.TaskCompletionSource.SetException(
                                                        new TimeoutException("ValidUntilUTC is less than current time"));
                                                }
                                            }
                                            catch (LuaException e)
                                            {
                                                Trace.TraceError(e.ToString());
                                            }
                                        }, serializedResult, TaskCreationOptions.PreferFairness);
                                    }
                                }
                            }
                            catch (TaskCanceledException)
                            {
                            } // Это исключение возникнет при отмене ReadLineAsync через Cancellation Token
                            catch (IOException e)
                            {
                                Trace.TraceError(e.ToString());
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceInformation("Response task is cancelling");
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                        _cancellationTokenSource.Cancel();
                        throw new AggregateException("Unhandled exception in background task", e);
                    }
                    finally
                    {
                        lock (_syncRoot)
                        {
                            if (_commandClient != null)
                            {
                                _commandClient.Client.Shutdown(SocketShutdown.Both);
                                _commandClient.Close();
                                _commandClient = null; // У нас два потока работают с одним сокетом, но только один из них должен его закрыть !
                                Trace.WriteLine("Response channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            // Callback Task
            _eventReceiverTask = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        // reconnection loop
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            Trace.WriteLine("Connecting on callback channel... ");
                            EnsureConnectedClient(_cancellationTokenSource.Token);
                            // now we are connected
                            OnConnected(_options.CommandPort); // Оповещаем клиента что произошло подключение к Quik'у
                            _connectedMre.Set();

                            // here we have a connected TCP client
                            Trace.WriteLine("Callback channel connected");
                            try
                            {
                                using (var stream = new NetworkStream(_eventClient.Client))
                                using (var reader = new StreamReader(stream, Encoding.GetEncoding(1251)))
                                {
                                    while (!_cancellationTokenSource.IsCancellationRequested)
                                    {
                                        var readLineTask = reader.ReadLineAsync();
                                        var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);

                                        if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                        {
                                            break;
                                        }

                                        Trace.Assert(readLineTask.Status == TaskStatus.RanToCompletion);
                                        var serializedEvent = readLineTask.Result;
                                        if (serializedEvent == null)
                                            throw new IOException("Lua returned an empty response or closed the connection");

                                        try
                                        {
                                            var @event = _jsonSerializer.Deserialize<IEvent>(serializedEvent);
                                            // it is a callback message
                                            await _eventChannel.Writer.WriteAsync(@event, _cancellationTokenSource.Token);
                                        }
                                        catch (Exception e) // deserialization exception is possible
                                        {
                                            Trace.TraceError(e.ToString());
                                        }
                                    }
                                }
                            }
                            catch (IOException e)
                            {
                                Trace.TraceError(e.ToString());
                                // handled exception will cause reconnect in the outer loop
                                _connectedMre.Reset();
                                OnDisconnected();
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceInformation("Callback task is cancelling");
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                        _cancellationTokenSource.Cancel();
                        throw new AggregateException("Unhandled exception in background task", e);
                    }
                    finally
                    {
                        lock (_syncRoot)
                        {
                            if (_eventClient != null)
                            {
                                _eventClient.Client.Shutdown(SocketShutdown.Both);
                                _eventClient.Close();
                                _eventClient = null;
                                Trace.WriteLine("Callback channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _eventInvokerTask = Task.Factory.StartNew(async () =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var @event = await _eventChannel.Reader.ReadAsync(_cancellationTokenSource.Token);
                            try
                            {
                                _eventInvoker.Invoke(@event);
                            }
                            catch (Exception e) // 
                            {
                                Trace.TraceError($"Error in event handler for {@event.Name}:\n" + e);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public bool IsConnected()
        {
            return (_commandClient != null && _commandClient.Connected && _commandClient.Client.IsConnected())
                   && (_eventClient != null && _eventClient.Connected && _eventClient.Client.IsConnected());
        }

        private void EnsureConnectedClient(CancellationToken ct)
        {
            lock (_syncRoot)
            {
                var attempt = 0;
                if (!(_commandClient != null && _commandClient.Connected && _commandClient.Client.IsConnected()))
                {
                    var connected = false;
                    while (!connected)
                    {
                        ct.ThrowIfCancellationRequested();
                        try
                        {
                            _commandClient = new TcpClient
                            {
                                ExclusiveAddressUse = true,
                                NoDelay = true
                            };
                            _commandClient.Connect(_options.Host, _options.CommandPort);
                            connected = true;
                        }
                        catch
                        {
                            attempt++;
                            Thread.Sleep(100);
                            if (attempt % 10 == 0) Trace.WriteLine($"Trying to connect... {attempt}");
                        }
                    }
                }

                if (!(_eventClient != null && _eventClient.Connected && _eventClient.Client.IsConnected()))
                {
                    var connected = false;
                    while (!connected)
                    {
                        ct.ThrowIfCancellationRequested();
                        try
                        {
                            _eventClient = new TcpClient
                            {
                                ExclusiveAddressUse = true,
                                NoDelay = true
                            };
                            _eventClient.Connect(_options.Host, _options.EventPort);
                            connected = true;
                        }
                        catch
                        {
                            attempt++;
                            Thread.Sleep(100);
                            if (attempt % 10 == 0) Trace.WriteLine($"Trying to connect... {attempt}");
                        }
                    }
                }
            }
        }

        public async Task<TResult> SendAsync<TResult>(ICommand command, TimeSpan? timeout = null)
            where TResult : class, IResult
        {
            if (timeout == null)
                timeout = _options.SendCommandTimeout;

            var task = _connectedMre.WaitAsync();
            if (timeout > TimeSpan.Zero)
            {
                var timeoutTask = Task.Delay(timeout.Value);
                if (await Task.WhenAny(task, timeoutTask).ConfigureAwait(false) == timeoutTask)
                    throw new TimeoutException("Send operation timed out");
            }
            else
            {
                await task.ConfigureAwait(false);
            }

            var tcs = new TaskCompletionSource<IResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var ctRegistration = default(CancellationTokenRegistration);

            command.Id = _idProvider.GetUniqueCommandId();

            if (timeout > TimeSpan.Zero)
            {
                var ct = new CancellationTokenSource();
                ctRegistration = ct.Token.Register(() =>
                {
                    tcs.TrySetException(new TimeoutException("Send operation timed out"));
                    _pendingResults.TryRemove(command.Id, out var _);
                }, false);

                ct.CancelAfter(timeout.Value);
            }

            _pendingResults[command.Id] = new PendingResult(command, typeof(TResult), tcs);
            // add to queue after responses dictionary
            _commandQueue.Add(command);

            try
            {
                var result = await tcs.Task.ConfigureAwait(false);
                
                if (result is TResult typedResult)
                {
                    return typedResult;
                }
                else
                {
                    throw new QuikException($"Result type mismatch. Provided: '{result?.GetType()}', but expected: '{typeof(TResult)}'.");
                }
            }
            finally
            {
                if (timeout > TimeSpan.Zero)
                {
                    ctRegistration.Dispose();
                }
            }
        }
    }
}