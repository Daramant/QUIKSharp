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
using QuikSharp.QuikEvents;
using QuikSharp.Messages;
using QuikSharp.Json.Serializers;
using QuikSharp.Extensions;
using QuikSharp.Exceptions;

namespace QuikSharp.QuikService
{
    /// <summary>
    ///
    /// </summary>
    public sealed class QuikService : IQuikService
    {
        private readonly AsyncManualResetEvent _connectedMre = new AsyncManualResetEvent();

        private readonly IQuikEventsInvoker _quikEventsInvoker;
        private readonly IJsonSerializer _jsonSerializer;

        private readonly IPAddress _host;
        private readonly int _responsePort;
        private readonly int _callbackPort;

        static QuikService()
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
        }

        public QuikService(
            IQuikEventsInvoker quikEventsInvoker, 
            IJsonSerializer jsonSerializer,
            QuikServiceOptions options)
        {
            _quikEventsInvoker = quikEventsInvoker;
            _jsonSerializer = jsonSerializer;

            _host = IPAddress.Parse(options.Host);
            _responsePort = options.ResponsePort;
            _callbackPort = options.CallbackPort;
        }

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
        private readonly BlockingCollection<IRequest> RequestQueue = new BlockingCollection<IRequest>();

        /// <summary>
        /// If received message has a correlation id then use its Data to SetResult on TCS and remove the TCS from the dic
        /// </summary>
        private readonly ConcurrentDictionary<long, PendingResponse> PendingResponses = new ConcurrentDictionary<long, PendingResponse>();

        /// <summary>
        ///
        /// </summary>
        public void Stop()
        {
            if (!IsStarted) return;
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
                                    IRequest request = null;
                                    try
                                    {
                                        // BLOCKING
                                        request = RequestQueue.Take(_cancellationTokenSource.Token);
                                        var serializedRequest = _jsonSerializer.Serialize(request);
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
                                            var response = _jsonSerializer.Deserialize<IResponse>((string)serializedResponseObj);
                                            Trace.Assert(response.Id > 0);
                                            // it is a response message
                                            if (!PendingResponses.ContainsKey(response.Id))
                                                throw new ApplicationException("Unexpected correlation ID");
                                            
                                            PendingResponses.TryRemove(response.Id, out var pendingResponse);
                                            if (!pendingResponse.Request.ValidUntil.HasValue || pendingResponse.Request.ValidUntil >= DateTime.UtcNow)
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
                            _quikEventsInvoker.OnConnectedToQuik(_responsePort); // Оповещаем клиента что произошло подключение к Quik'у
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
                                        var @event = _jsonSerializer.Deserialize<IEvent>(callback);
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
                                _quikEventsInvoker.OnDisconnectedFromQuik();
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
                                ProcessCallbackMessage(@event);
                            }
                            catch (Exception e) // 
                            {
                                Trace.TraceError($"Error in callback event handler for {@event.Name}:\n" + e);
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

        private void ProcessCallbackMessage(IEvent @event)
        {
            if (@event == null)
            {
                Trace.WriteLine("Trace: ProcessCallbackMessage(). message = NULL");
                throw new ArgumentNullException(nameof(@event));
            }

            var parsed = Enum.TryParse(@event.Name, ignoreCase: true, out EventName eventName);
            if (parsed)
            {
                // TODO use as instead of assert+is+cast
                switch (eventName)
                {
                    case EventName.AccountBalance:
                        Trace.Assert(@event is Event<AccountBalance>);
                        var accountBalance = ((Event<AccountBalance>) @event).Data;
                        _quikEventsInvoker.OnAccountBalance(accountBalance);
                        break;

                    case EventName.AccountPosition:
                        Trace.Assert(@event is Event<AccountPosition>);
                        var accPos = ((Event<AccountPosition>) @event).Data;
                        _quikEventsInvoker.OnAccountPosition(accPos);
                        break;

                    case EventName.AllTrade:
                        Trace.Assert(@event is Event<AllTrade>);
                        var allTrade = ((Event<AllTrade>) @event).Data;
                        allTrade.LuaTimeStamp = @event.CreatedTime;
                        _quikEventsInvoker.OnAllTrade(allTrade);
                        break;

                    case EventName.CleanUp:
                        Trace.Assert(@event is Event<string>);
                        _quikEventsInvoker.OnCleanUp();
                        break;

                    case EventName.Close:
                        Trace.Assert(@event is Event<string>);
                        _quikEventsInvoker.OnClose();
                        break;

                    case EventName.Connected:
                        Trace.Assert(@event is Event<string>);
                        _quikEventsInvoker.OnConnected();
                        break;

                    case EventName.DepoLimit:
                        Trace.Assert(@event is Event<DepoLimitEx>);
                        var dLimit = ((Event<DepoLimitEx>) @event).Data;
                        _quikEventsInvoker.OnDepoLimit(dLimit);
                        break;

                    case EventName.DepoLimitDelete:
                        Trace.Assert(@event is Event<DepoLimitDelete>);
                        var dLimitDel = ((Event<DepoLimitDelete>) @event).Data;
                        _quikEventsInvoker.OnDepoLimitDelete(dLimitDel);
                        break;

                    case EventName.Disconnected:
                        Trace.Assert(@event is Event<string>);
                        _quikEventsInvoker.OnDisconnected();
                        break;

                    case EventName.Firm:
                        Trace.Assert(@event is Event<Firm>);
                        var frm = ((Event<Firm>) @event).Data;
                        _quikEventsInvoker.OnFirm(frm);
                        break;

                    case EventName.FuturesClientHolding:
                        Trace.Assert(@event is Event<FuturesClientHolding>);
                        var futPos = ((Event<FuturesClientHolding>) @event).Data;
                        _quikEventsInvoker.OnFuturesClientHolding(futPos);
                        break;

                    case EventName.FuturesLimitChange:
                        Trace.Assert(@event is Event<FuturesLimits>);
                        var futLimit = ((Event<FuturesLimits>) @event).Data;
                        _quikEventsInvoker.OnFuturesLimitChange(futLimit);
                        break;

                    case EventName.FuturesLimitDelete:
                        Trace.Assert(@event is Event<FuturesLimitDelete>);
                        var limDel = ((Event<FuturesLimitDelete>) @event).Data;
                        _quikEventsInvoker.OnFuturesLimitDelete(limDel);
                        break;

                    case EventName.Init:
                        // Этот callback никогда не будет вызван так как на момент получения вызова OnInit в lua скрипте
                        // соединение с библиотекой QuikSharp не будет еще установлено. То есть этот callback не имеет смысла.
                        break;

                    case EventName.MoneyLimit:
                        Trace.Assert(@event is Event<MoneyLimitEx>);
                        var mLimit = ((Event<MoneyLimitEx>) @event).Data;
                        _quikEventsInvoker.OnMoneyLimit(mLimit);
                        break;

                    case EventName.MoneyLimitDelete:
                        Trace.Assert(@event is Event<MoneyLimitDelete>);
                        var mLimitDel = ((Event<MoneyLimitDelete>) @event).Data;
                        _quikEventsInvoker.OnMoneyLimitDelete(mLimitDel);
                        break;

                    case EventName.NegDeal:
                        break;

                    case EventName.NegTrade:
                        break;

                    case EventName.Order:
                        Trace.Assert(@event is Event<Order>);
                        var ord = ((Event<Order>) @event).Data;
                        ord.LuaTimeStamp = @event.CreatedTime;
                        _quikEventsInvoker.OnOrder(ord);
                        break;

                    case EventName.Param:
                        Trace.Assert(@event is Event<Param>);
                        var data = ((Event<Param>) @event).Data;
                        _quikEventsInvoker.OnParam(data);
                        break;

                    case EventName.Quote:
                        Trace.Assert(@event is Event<OrderBook>);
                        var ob = ((Event<OrderBook>) @event).Data;
                        ob.LuaTimeStamp = @event.CreatedTime;
                        _quikEventsInvoker.OnQuote(ob);
                        break;

                    case EventName.Stop:
                        Trace.Assert(@event is Event<string>);
                        _quikEventsInvoker.OnStop(int.Parse(((Event<string>) @event).Data));
                        break;

                    case EventName.StopOrder:
                        Trace.Assert(@event is Event<StopOrder>);
                        StopOrder stopOrder = ((Event<StopOrder>) @event).Data;
                        _quikEventsInvoker.OnStopOrder(stopOrder);
                        break;

                    case EventName.Trade:
                        Trace.Assert(@event is Event<Trade>);
                        var trade = ((Event<Trade>) @event).Data;
                        trade.LuaTimeStamp = @event.CreatedTime;
                        _quikEventsInvoker.OnTrade(trade);
                        break;

                    case EventName.TransReply:
                        Trace.Assert(@event is Event<TransactionReply>);
                        var trReply = ((Event<TransactionReply>) @event).Data;
                        trReply.LuaTimeStamp = @event.CreatedTime;
                        _quikEventsInvoker.OnTransReply(trReply);
                        break;

                    case EventName.NewCandle:
                        Trace.Assert(@event is Event<Candle>);
                        var candle = ((Event<Candle>) @event).Data;
                        _quikEventsInvoker.OnNewCandle(candle);
                        break;

                    default:
                        throw new NotSupportedException($"EventName: '{eventName}' not supported.");
                }
            }
            else
            {
                switch (@event.Name)
                {
                    // an error from an event not request (from req is caught is response loop)
                    case "lua_error":
                        Trace.Assert(@event is Event<string>);
                        Trace.TraceError(((Event<string>) @event).Data);
                        break;

                    default:
                        throw new InvalidOperationException("Unknown command in a message: " + @event.Name);
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
        internal int GetUniqueTransactionId()
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

        public async Task<TResponse> Send<TResponse>(IRequest request, int timeout = 0)
            where TResponse : class, IResponse, new()
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

            var tcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
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

            PendingResponses[request.Id] = new PendingResponse(request, typeof(TResponse), tcs);
            // add to queue after responses dictionary
            RequestQueue.Add(request);
            IResponse response;

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