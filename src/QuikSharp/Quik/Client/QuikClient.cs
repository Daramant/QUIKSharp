// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using QuikSharp.Messages;
using QuikSharp.Extensions;
using QuikSharp.Exceptions;
using QuikSharp.Quik.Events;
using QuikSharp.Providers;
using QuikSharp.Serialization;
using Microsoft.Extensions.Logging;

namespace QuikSharp.Quik.Client
{
    /// <summary>
    ///
    /// </summary>
    public sealed class QuikClient : IQuikClient
    {
        private readonly IEventInvoker _eventInvoker;
        private readonly ISerializer _serializer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPendingResultContainer _pendingResultContainer;
        private readonly QuikClientOptions _options;
        private readonly ILogger<QuikClient> _logger;

        private IQuik _quik;

        private readonly SemaphoreSlim _commandClientSemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _eventClientSemaphoreSlim = new SemaphoreSlim(1, 1);

        private TcpClient _commandClient;
        private TcpClient _eventClient;

        private Task _commandTask;
        private Task _resultTask;
        private Task _eventReceiverTask;
        private Task _eventInvokerTask;

        private readonly Channel<string> _eventChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
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
        private readonly BlockingCollection<ICommand> _commandEnvelopeQueue = new BlockingCollection<ICommand>();

        public ClientState State { get; private set; } = ClientState.Stopped;

        public QuikClient(
            IEventInvoker eventInvoker,
            ISerializer serializer,
            IDateTimeProvider dateTimeProvider,
            IPendingResultContainer pendingResultContainer,
            QuikClientOptions options,
            ILogger<QuikClient> logger)
        {
            _eventInvoker = eventInvoker;
            _serializer = serializer;
            _dateTimeProvider = dateTimeProvider;
            _pendingResultContainer = pendingResultContainer;
            _options = options;
            _logger = logger;
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

        /// <inheritdoc/>
        public void SetEventSender(IQuik quik)
        {
            _quik = quik;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (State == ClientState.Starting || State == ClientState.Started)
                return;

            State = ClientState.Starting;

            _cancellationTokenSource = new CancellationTokenSource();
            _taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _cancellationTokenRegistration = _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetResult(true));

            _commandTask = Task.Factory.StartNew(SendCommandAction,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _resultTask = Task.Factory.StartNew(ReceiveResultAction,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _eventReceiverTask = Task.Factory.StartNew(ReceiveEventAction,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _eventInvokerTask = Task.Factory.StartNew(InvokeEventAction,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            State = ClientState.Started;
        }

        private async Task SendCommandAction()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.LogTrace("Подключение к терминалу Quik для отправки команд...");
                    _commandClient = await ReconnectIfMissingConnectionAsync(_commandClient, _commandClientSemaphoreSlim, _cancellationTokenSource.Token);
                    _logger.LogTrace("Подключение к терминалу Quik для отправки команд установлено.");
                    
                    try
                    {
                        using (var stream = new NetworkStream(_commandClient.Client))
                        using (var writer = new StreamWriter(stream))
                        {
                            while (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                var command = _commandEnvelopeQueue.Take(_cancellationTokenSource.Token);

                                if (!_pendingResultContainer.TryGet(command.Id, out var pendingResult))
                                {
                                    _logger.LogWarning($"Среди находящихся в ожидании результатов команд нет результата для команды с идентификатором: {command.Id}.");
                                }

                                var utcNow = _dateTimeProvider.UtcNow;

                                if (command.ValidUntil < utcNow)
                                {
                                    if (_pendingResultContainer.TryRemove(command.Id, out pendingResult))
                                    {
                                        pendingResult.TaskCompletionSource.SetException(
                                            new CommandTimeoutException($"Результат выполнения команды с идентификатором: {command.Id} получен в: '{utcNow}' после крайнего срока: '{pendingResult.Command.ValidUntil}'."));
                                    }
                                    else
                                    {
                                        _logger.LogWarning($"Среди находящихся в ожидании результатов команд нет результата для команды с идентификатором: {command.Id}.");
                                    }
                                }
                                else
                                {
                                    var serializedCommandEnvelope = _serializer.Serialize(command);

                                    try
                                    {
                                        await writer.WriteLineAsync(serializedCommandEnvelope);
                                        await writer.FlushAsync();
                                    }
                                    catch (IOException)
                                    {
                                        // При ошибке ввода/вывода (например при перезапуске терминала Quik или остановке скрипта)
                                        // возвращаем команду обратно в очередь, чтобы отправить повторно после установки
                                        // соединения с терминалом Quik.
                                        _commandEnvelopeQueue.Add(command);

                                        throw;
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        _logger.LogError(e, "Исключение при отправке команды в терминал Quik.");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                _logger.LogTrace(e, "Отправка команд в терминал Quik остановлена.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Исключение при отправке команды.");
                _cancellationTokenSource.Cancel();

                throw new QuikSharpException("Исключение при отправке команд.", e);
            }
            finally
            {
                _commandClient = await CloseClientAsync(_commandClient, _commandClientSemaphoreSlim);
            }
        }

        private async Task ReceiveResultAction()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.LogTrace("Подключение к терминалу Quik для получения результатов команд...");
                    _commandClient = await ReconnectIfMissingConnectionAsync(_commandClient, _commandClientSemaphoreSlim, _cancellationTokenSource.Token);
                    _logger.LogTrace("Подключение к терминалу Quik для получения результатов команд установлено.");

                    try
                    {
                        using (var stream = new NetworkStream(_commandClient.Client))
                        using (var reader = new StreamReader(stream, _options.Encoding))
                        {
                            while (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                var readLineTask = reader.ReadLineAsync();
                                var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);
                                if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                {
                                    break;
                                }

                                var serializedResult = readLineTask.Result;

                                if (serializedResult == null)
                                    throw new QuikSharpException("От терминала Quik получен пустой ответ.");

                                // No IO exceptions possible for response, move its processing
                                // to the threadpool and wait for the next message
                                // A new task here gives c.30% boost for full TransactionSpec echo

                                var doNotAwaitMe = Task.Factory.StartNew(serializedResultObj =>
                                {
                                    try
                                    {
                                        ProcessResultEnvelope((string)serializedResultObj);
                                    }
                                    catch (LuaException e)
                                    {
                                        _logger.LogError(e, "Исключение при обработке результата команды.");
                                    }
                                }, serializedResult, TaskCreationOptions.PreferFairness);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        _logger.LogError(e, "Исключение при получении результата команды от терминал Quik.");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                _logger.LogTrace(e, "Получение результов команд от терминала Quik остановлено.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Исключение при получении результата команды.");
                _cancellationTokenSource.Cancel();

                throw new QuikSharpException("Исключение при получении результатов команд.", e);
            }
            finally
            {
                _commandClient = await CloseClientAsync(_commandClient, _commandClientSemaphoreSlim);
            }
        }

        private async Task ReceiveEventAction()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.LogTrace("Подключение к терминалу Quik для получения событий...");
                    _eventClient = await ReconnectIfMissingConnectionAsync(_eventClient, _eventClientSemaphoreSlim, _cancellationTokenSource.Token);
                    _logger.LogTrace("Подключение к терминалу Quik для получения событий установлено.");
                    
                    OnConnected(_options.CommandPort);

                    try
                    {
                        using (var stream = new NetworkStream(_eventClient.Client))
                        using (var reader = new StreamReader(stream, _options.Encoding))
                        {
                            while (!_cancellationTokenSource.IsCancellationRequested)
                            {
                                var readLineTask = reader.ReadLineAsync();
                                var completedTask = await Task.WhenAny(readLineTask, _taskCompletionSource.Task).ConfigureAwait(false);

                                if (completedTask == _taskCompletionSource.Task || _cancellationTokenSource.IsCancellationRequested)
                                {
                                    break;
                                }

                                var serializedEvent = readLineTask.Result;
                                if (serializedEvent == null)
                                    throw new QuikSharpException("Терминал Quik вернул пустой ответ.");

                                await _eventChannel.Writer.WriteAsync(serializedEvent, _cancellationTokenSource.Token);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        _logger.LogError(e, "Исключение при получении событий от терминал Quik.");
                        
                        OnDisconnected();
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                _logger.LogTrace(e, "Получение событий от терминала Quik остановлено.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Исключение при получении события.");
                _cancellationTokenSource.Cancel();

                throw new QuikSharpException("Исключение при получении событий.", e);
            }
            finally
            {
                _eventClient = await CloseClientAsync(_eventClient, _eventClientSemaphoreSlim);
                OnDisconnected();
            }
        }

        private async Task InvokeEventAction()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var serializedEvent = await _eventChannel.Reader.ReadAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
                    
                    try
                    {
                        var envelope = _serializer.DeserializeEventEnvelope(serializedEvent);
                        _eventInvoker.Invoke(envelope.Body);
                    }
                    catch (Exception e)
                    {
                        e.Data["Event"] = serializedEvent;
                        _logger.LogError(e, "Исключение при обработке события.");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                _logger.LogTrace(e, "Обработка событий от терминала Quik остановлена.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Исключение при обработке событий.");
                _cancellationTokenSource.Cancel();

                throw new QuikSharpException("Исключение при обработке событий.", e);
            }
        }

        private void ProcessResultEnvelope(string data)
        {
            var envelope = _serializer.DeserializeResultEnvelope(data);

            if (!_pendingResultContainer.TryRemove(envelope.Header.CommandId, out var pendingResult))
            {
                _logger.LogWarning($"Среди находящихся в ожидании результатов команд нет результата для команды с идентификатором: {envelope.Header.CommandId}.");
                return;
            }

            if (envelope.Header.Status == ResultStatus.Ok)
            {
                var utcNow = _dateTimeProvider.UtcNow;
                if (pendingResult.Command.ValidUntil < utcNow)
                {
                    pendingResult.TaskCompletionSource.SetException(
                        new CommandTimeoutException($"Результат выполнения команды с идентификатором: {envelope.Header.CommandId} получен в: {utcNow} после крайнего срока: {pendingResult.Command.ValidUntil}."));
                }
                else
                {
                    pendingResult.TaskCompletionSource.SetResult(envelope.Body);
                }
            }
            else
            {
                pendingResult.TaskCompletionSource.SetException(
                    new LuaException($"Не удалось выполнить команду с идентификатором: {envelope.Header.CommandId}. Детали: '{((Result<string>)envelope.Body).Data}'."));
            }
        }

        public bool IsConnected()
        {
            return (_commandClient != null && _commandClient.Client.IsConnected())
                   && (_eventClient != null && _eventClient.Client.IsConnected());
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            if (State == ClientState.Stopping || State == ClientState.Stopped)
                return;

            State = ClientState.Stopping;

            _cancellationTokenSource.Cancel();
            _cancellationTokenRegistration.Dispose();

            try
            {
                var stoppingTasks = Task.WhenAll(_commandTask, _resultTask, _eventReceiverTask, _eventInvokerTask);
                await Task.WhenAny(stoppingTasks, Task.Delay(_options.StopTimeout));

                if (!stoppingTasks.IsCompleted)
                {
                    _logger.LogWarning($"Не все задачи были отменены после остановки клиента (CommandTask: {_commandTask.Status}, ResultTask: {_resultTask.Status}, EventRecevierTask: {_eventReceiverTask.Status}, EventInvokerTask: {_eventInvokerTask.Status}, Timeout: {_options.StopTimeout}).");
                }
            }
            finally
            {
                _pendingResultContainer.CancelAll();
            }

            State = ClientState.Stopped;
        }

        /// <inheritdoc/>
        public async Task WaitForConnectionAsync()
        {
            _commandClient = await ReconnectIfMissingConnectionAsync(_commandClient, _commandClientSemaphoreSlim, _cancellationTokenSource.Token);
            _eventClient = await ReconnectIfMissingConnectionAsync(_eventClient, _eventClientSemaphoreSlim, _cancellationTokenSource.Token);
        }

        /// <inheritdoc/>
        public async Task<TResult> SendAsync<TResult>(ICommand command, TimeSpan? timeout = null)
            where TResult : class, IResult
        {
            if (timeout == null)
                timeout = _options.SendCommandTimeout;

            var taskCompletionSource = new TaskCompletionSource<IResult>(TaskCreationOptions.RunContinuationsAsynchronously);

            var cancellationTokenSource = default(CancellationTokenSource);
            var cancellationTokenRegistration = default(CancellationTokenRegistration);

            if (timeout > TimeSpan.Zero)
            {
                cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenRegistration = cancellationTokenSource.Token.Register(() =>
                {
                    taskCompletionSource.TrySetException(new CommandTimeoutException("Send operation timed out"));
                    _pendingResultContainer.Remove(command.Id);
                }, useSynchronizationContext: false);

                cancellationTokenSource.CancelAfter(timeout.Value);
            }

            _pendingResultContainer.Add(command.Id, new PendingResult(command, typeof(TResult), taskCompletionSource));
            _commandEnvelopeQueue.Add(command);

            try
            {
                var result = await taskCompletionSource.Task.ConfigureAwait(false);

                if (result is TResult typedResult)
                {
                    return typedResult;
                }
                else
                {
                    throw new QuikSharpException($"Result type mismatch. Provided: '{result?.GetType()}', but expected: '{typeof(TResult)}'.");
                }
            }
            finally
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
        }

        private async Task<TcpClient> ReconnectIfMissingConnectionAsync(TcpClient tcpClient, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
        {
            if (tcpClient?.Client.IsConnected() == true)
                return tcpClient;

            await semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                if (tcpClient?.Client.IsConnected() == true)
                    return tcpClient;

                var attemptCount = 0;
                var maxAttemptCount = _options.ConnectionAttemptCount;
                
                if (tcpClient == null)
                {
                    tcpClient = new TcpClient
                    {
                        ExclusiveAddressUse = true,
                        NoDelay = true
                    };
                }

                var isConnected = false;
                while (!isConnected)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        await _commandClient.ConnectAsync(_options.Host, _options.CommandPort);
                        isConnected = true;
                    }
                    catch
                    {
                        attemptCount++;

                        if (maxAttemptCount > 0 && attemptCount >= maxAttemptCount)
                            throw new QuikSharpException($"Не удалось подключиться к '{_options.Host}:{_options.CommandPort}' за {maxAttemptCount} попыток.");

                        await Task.Delay(_options.ConnectionAttemptTimeout);

                        if (attemptCount % 10 == 0)
                        {
                            _logger.LogInformation($"Подключение к терминалу Quik. Попытка: {attemptCount}...");
                        }
                    }
                }

                return tcpClient;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task<TcpClient> CloseClientAsync(TcpClient tcpClient, SemaphoreSlim semaphoreSlim)
        {
            if (tcpClient == null)
                return tcpClient;

            await semaphoreSlim.WaitAsync();
            try
            {
                if (tcpClient == null)
                    return tcpClient;

                tcpClient.Client.Shutdown(SocketShutdown.Both);
                tcpClient.Close();
                tcpClient = null; // У нас два потока работают с одним сокетом, но только один из них должен его закрыть.

                return tcpClient;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}