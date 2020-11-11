// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

namespace QuikSharp.QuikClient
{
    /// <summary>
    ///
    /// </summary>
    public sealed class QuikClient : IQuikClient
    {
        private readonly AsyncManualResetEvent _connectedMre = new AsyncManualResetEvent();

        private readonly IQuikEventHandler _quikEventHandler;
        private readonly IQuikJsonSerializer _quikJsonSerializer;

        private readonly IPAddress _host;
        private readonly int _responsePort;
        private readonly int _callbackPort;

        static QuikClient()
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
        }

        public QuikClient(
            IQuikEventHandler quikEventHandler, 
            IQuikJsonSerializer quikJsonSerializer,
            QuikClientOptions options)
        {
            _quikEventHandler = quikEventHandler;
            _quikJsonSerializer = quikJsonSerializer;

            _host = IPAddress.Parse(options.Host);
            _responsePort = options.ResponsePort;
            _callbackPort = options.CallbackPort;
        }

        #region Events

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp успешно подключилась к Quik'у
        /// </summary>
        public event InitHandler Connected;

        private void OnConnected(int port)
        {
            Connected?.Invoke(port);
        }

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp была отключена от Quik'а
        /// </summary>
        public event VoidHandler Disconnected;

        private void OnDisconnected()
        {
            Disconnected?.Invoke();
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления и при закрытии терминала QUIK.
        /// </summary>
        public event StopHandler Stop;

        private void OnStop(int signal)
        {
            Stop?.Invoke(signal);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// info.exe file path
        /// </summary>
        public string WorkingFolder { get; private set; }

        internal const int UniqueIdOffset = 0;
        internal readonly string SessionId = DateTime.Now.ToString("yyMMddHHmmss");
        internal MemoryMappedFile mmf;
        internal MemoryMappedViewAccessor accessor;

        
        private TcpClient _responseClient;
        private TcpClient _callbackClient;

        private readonly object _syncRoot = new object();

        private Task _requestTask;
        private Task _responseTask;
        private Task _callbackReceiverTask;
        private Task _callbackInvokerTask;

        private Channel<IEvent> _receivedCallbacksChannel =
            Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true
            });

        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;
        private CancellationTokenRegistration _cancellationTokenRegistration;

        /// <summary>
        /// Current correlation id. Use Interlocked.Increment to get a new id.
        /// </summary>
        private static int _correlationId;

        /// <summary>
        /// IQuickCalls functions enqueue a message and return a task from TCS
        /// </summary>
        private readonly BlockingCollection<ICommand> RequestQueue = new BlockingCollection<ICommand>();

        /// <summary>
        /// If received message has a correlation id then use its Data to SetResult on TCS and remove the TCS from the dic
        /// </summary>
        internal readonly ConcurrentDictionary<long, PendingResult> PendingResponses = new ConcurrentDictionary<long, PendingResult>();

        /// <summary>
        ///
        /// </summary>
        public Task StopAsync()
        {
            if (!IsStarted) return Task.CompletedTask;
            IsStarted = false;
            _cancellationTokenSource.Cancel();
            _cancellationTokenRegistration.Dispose();

            try
            {
                // here all tasks must exit gracefully
                var isCleanExit = Task.WaitAll(new[] {_requestTask, _responseTask, _callbackReceiverTask}, 5000);
                Trace.Assert(isCleanExit, "All tasks must finish gracefully after cancellation token is cancelled!");
            }
            finally
            {
                // cancel responses to release waiters
                foreach (var responseKey in PendingResponses.Keys.ToList())
                {
                    if (PendingResponses.TryRemove(responseKey, out var pendingResponse))
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
            if (IsStarted) return;
            IsStarted = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _cancellationTokenRegistration = _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetResult(true));

            // Request Task
            _requestTask = Task.Factory.StartNew(() =>
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
                                var stream = new NetworkStream(_responseClient.Client);
                                var writer = new StreamWriter(stream);
                                while (!_cancellationTokenSource.IsCancellationRequested)
                                {
                                    ICommand request = null;
                                    try
                                    {
                                        // BLOCKING
                                        request = RequestQueue.Take(_cancellationTokenSource.Token);
                                        var serializedRequest = _quikJsonSerializer.Serialize(request);
                                        //Trace.WriteLine("Request: " + request);
                                        // scenario: Quik is restarted or script is stopped
                                        // then writer must throw and we will add a message back
                                        // then we will iterate over messages and cancel expired ones
                                        if (!request.ValidUntil.HasValue || request.ValidUntil >= DateTime.UtcNow)
                                        {
                                            writer.WriteLine(request);
                                            writer.Flush();
                                        }
                                        else
                                        {
                                            Trace.Assert(request.Id > 0, "All requests must have correlation id");
                                            PendingResponses[request.Id]
                                                .TaskCompletionSource.SetException(
                                                    new TimeoutException("ValidUntilUTC is less than current time"));
                                            
                                            PendingResponses.TryRemove(request.Id, out var _);
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
                                        if (request != null)
                                        {
                                            RequestQueue.Add(request);
                                        }

                                        break;
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
                        throw new AggregateException("Unhandled exception in background task", e);
                    }
                    finally
                    {
                        lock (_syncRoot)
                        {
                            if (_responseClient != null)
                            {
                                _responseClient.Client.Shutdown(SocketShutdown.Both);
                                _responseClient.Close();
                                _responseClient =
                                    null; // У нас два потока работают с одним сокетом, но только один из них должен его закрыть !
                                Trace.WriteLine("Response channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            // Response Task
            _responseTask = Task.Factory.StartNew(async () =>
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
                                var stream = new NetworkStream(_responseClient.Client);
                                var reader = new StreamReader(stream, Encoding.GetEncoding(1251)); //true
                                while (!_cancellationTokenSource.IsCancellationRequested)
                                {
                                    var readLineTask = reader.ReadLineAsync();
                                    var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);
                                    if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                    {
                                        break;
                                    }

                                    Trace.Assert(readLineTask.Status == TaskStatus.RanToCompletion);
                                    var serializedResponse = readLineTask.Result;
                                    if (serializedResponse == null)
                                    {
                                        throw new IOException("Lua returned an empty response or closed the connection");
                                    }

                                    // No IO exceptions possible for response, move its processing
                                    // to the threadpool and wait for the next message
                                    // A new task here gives c.30% boost for full TransactionSpec echo

                                    // ReSharper disable once UnusedVariable
                                    var doNotAwaitMe = Task.Factory.StartNew(serializedResponseObj =>
                                    {
                                        //var r = response;
                                        //Trace.WriteLine("Response:" + response);
                                        try
                                        {
                                            var response = _quikJsonSerializer.Deserialize<IResult>((string)serializedResponseObj);
                                            Trace.Assert(response.Id > 0);
                                            // it is a response message
                                            if (!PendingResponses.ContainsKey(response.Id))
                                                throw new ApplicationException("Unexpected correlation ID");
                                            
                                            PendingResponses.TryRemove(response.Id, out var pendingResponse);
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
                                    }, serializedResponse, TaskCreationOptions.PreferFairness);
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
                            if (_responseClient != null)
                            {
                                _responseClient.Client.Shutdown(SocketShutdown.Both);
                                _responseClient.Close();
                                _responseClient = null; // У нас два потока работают с одним сокетом, но только один из них должен его закрыть !
                                Trace.WriteLine("Response channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            // Callback Task
            _callbackReceiverTask = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        // reconnection loop
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            Trace.WriteLine("Connecting on callback channel... ");
                            EnsureConnectedClient(_cancellationTokenSource.Token);
                            // now we are connected
                            OnConnected(_responsePort); // Оповещаем клиента что произошло подключение к Quik'у
                            _connectedMre.Set();

                            // here we have a connected TCP client
                            Trace.WriteLine("Callback channel connected");
                            try
                            {
                                var stream = new NetworkStream(_callbackClient.Client);
                                var reader = new StreamReader(stream, Encoding.GetEncoding(1251)); //true
                                while (!_cancellationTokenSource.IsCancellationRequested)
                                {
                                    var readLineTask = reader.ReadLineAsync();
                                    var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);

                                    if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                    {
                                        break;
                                    }

                                    Trace.Assert(readLineTask.Status == TaskStatus.RanToCompletion);
                                    var callback = readLineTask.Result;
                                    if (callback == null)
                                    {
                                        throw new IOException("Lua returned an empty response or closed the connection");
                                    }

                                    try
                                    {
                                        var @event = _quikJsonSerializer.Deserialize<IEvent>(callback);
                                        // it is a callback message
                                        await _receivedCallbacksChannel.Writer.WriteAsync(@event);
                                    }
                                    catch (Exception e) // deserialization exception is possible
                                    {
                                        Trace.TraceError(e.ToString());
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
                            if (_callbackClient != null)
                            {
                                _callbackClient.Client.Shutdown(SocketShutdown.Both);
                                _callbackClient.Close();
                                _callbackClient = null;
                                Trace.WriteLine("Callback channel disconnected");
                            }
                        }
                    }
                },
                CancellationToken.None, // NB we use the token for signalling, could use a simple TCS
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _callbackInvokerTask = Task.Factory.StartNew(async () =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var @event = await _receivedCallbacksChannel.Reader.ReadAsync(_cancellationTokenSource.Token);
                            try
                            {
                                _quikEventHandler.Handle(@event);
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
            return (_responseClient != null && _responseClient.Connected && _responseClient.Client.IsConnected())
                   && (_callbackClient != null && _callbackClient.Connected && _callbackClient.Client.IsConnected());
        }

        private void EnsureConnectedClient(CancellationToken ct)
        {
            lock (_syncRoot)
            {
                var attempt = 0;
                if (!(_responseClient != null && _responseClient.Connected && _responseClient.Client.IsConnected()))
                {
                    var connected = false;
                    while (!connected)
                    {
                        ct.ThrowIfCancellationRequested();
                        try
                        {
                            _responseClient = new TcpClient
                            {
                                ExclusiveAddressUse = true,
                                NoDelay = true
                            };
                            _responseClient.Connect(_host, _responsePort);
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

                if (!(_callbackClient != null && _callbackClient.Connected && _callbackClient.Client.IsConnected()))
                {
                    var connected = false;
                    while (!connected)
                    {
                        ct.ThrowIfCancellationRequested();
                        try
                        {
                            _callbackClient = new TcpClient
                            {
                                ExclusiveAddressUse = true,
                                NoDelay = true
                            };
                            _callbackClient.Connect(_host, _callbackPort);
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

        /// <summary>
        /// Generate a new unique ID for current session
        /// </summary>
        internal int GetNewUniqueId()
        {
            lock (_syncRoot)
            {
                var newId = Interlocked.Increment(ref _correlationId);
                // 2^31 = 2147483648
                // with 1 000 000 messages per second it will take more than
                // 35 hours to overflow => safe for use as TRANS_ID in SendTransaction
                // very weird stuff: Уникальный идентификационный номер заявки, значение от 1 до 2 294 967 294
                if (newId > 0)
                {
                    return newId;
                }

                _correlationId = 1;
                return 1;
            }
        }

        /// <summary>
        /// Get or Generate unique transaction ID for function SendTransaction()
        /// </summary>
        public int GetUniqueTransactionId()
        {
            if (mmf == null || accessor == null)
            {
                if (String.IsNullOrEmpty(WorkingFolder)) //WorkingFolder = Не определено. Создаем MMF в памяти
                {
                    mmf = MemoryMappedFile.CreateOrOpen("UniqueID", 4096);
                }
                else //WorkingFolder определен. Открываем MMF с диска
                {
                    string diskFileName = WorkingFolder + "\\" + "QUIKSharp.Settings";
                    try
                    {
                        mmf = MemoryMappedFile.CreateFromFile(diskFileName, FileMode.OpenOrCreate, "UniqueID", 4096);
                    }
                    catch
                    {
                        mmf = MemoryMappedFile.CreateOrOpen("UniqueID", 4096);
                    }
                }

                accessor = mmf.CreateViewAccessor();
            }

            int newId = accessor.ReadInt32(UniqueIdOffset);
            if (newId == 0)
            {
                newId = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
            }
            else
            {
                if (newId >= 2147483638)
                {
                    newId = 100;
                }
                newId++;
            }

            try
            {
                accessor.Write(UniqueIdOffset, newId);
            }
            catch (Exception er)
            {
                Console.WriteLine("Неудачная попытка записини нового ID в файл MMF: " + er.Message);
            }

            return newId;
        }

        /// <summary>
        /// Устанавливает стартовое значение для CorrelactionId.
        /// </summary>
        /// <param name="startCorrelationId">Стартовое значение.</param>
        public void InitializeCorrelationId(int startCorrelationId)
        {
            _correlationId = startCorrelationId;
        }

        internal string PrependWithSessionId(long id)
        {
            return SessionId + "." + id;
        }

        /// <summary>
        /// Default timeout to use for send operations if no specific timeout supplied.
        /// </summary>
        public TimeSpan DefaultSendTimeout { get; set; } = Timeout.InfiniteTimeSpan;

        public async Task<TResponse> SendAsync<TResponse>(ICommand request, int timeout = 0)
            where TResponse : class, IResult, new()
        {
            // use DefaultSendTimeout for default calls
            if (timeout == 0)
                timeout = (int) DefaultSendTimeout.TotalMilliseconds;

            var task = _connectedMre.WaitAsync();
            if (timeout > 0)
            {
                if (await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false) == task)
                {
                    // task completed within timeout, do nothing
                }
                else
                {
                    // timeout
                    throw new TimeoutException("Send operation timed out");
                }
            }
            else
            {
                await task.ConfigureAwait(false);
            }

            var tcs = new TaskCompletionSource<IResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var ctRegistration = default(CancellationTokenRegistration);

            request.Id = GetNewUniqueId();

            if (timeout > 0)
            {
                var ct = new CancellationTokenSource();
                ctRegistration = ct.Token.Register(() =>
                {
                    tcs.TrySetException(new TimeoutException("Send operation timed out"));
                    PendingResponses.TryRemove(request.Id, out var pendingResponse);
                }, false);

                ct.CancelAfter(timeout);
            }

            PendingResponses[request.Id] = new PendingResult(request, typeof(TResponse), tcs);
            // add to queue after responses dictionary
            RequestQueue.Add(request);
            IResult response;

            try
            {
                response = await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                if (timeout > 0)
                    ctRegistration.Dispose();
            }

            return (response as TResponse);
        }
    }
}