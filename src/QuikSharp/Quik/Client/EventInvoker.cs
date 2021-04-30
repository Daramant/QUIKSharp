using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Messages;
using QuikSharp.Quik.Events;
using QuikSharp.TypeConverters;
using System;
using System.Diagnostics;

namespace QuikSharp.Quik.Client
{
    public class EventInvoker : IEventInvoker
    {
        private readonly IQuikEventsInvoker _quikEventsInvoker;

        public EventInvoker(
            IQuikEventsInvoker quikEventsInvoker)
        {
            _quikEventsInvoker = quikEventsInvoker;
        }

        public void Invoke(IEvent @event)
        {
            if (@event == null)
            {
                Trace.WriteLine("Trace: ProcessCallbackMessage(). message = NULL");
                throw new ArgumentNullException(nameof(@event));
            }

            // TODO use as instead of assert+is+cast
            switch (@event.Name)
            {
                case EventName.AccountBalance:
                    Trace.Assert(@event is Event<AccountBalance>);
                    var accountBalance = ((Event<AccountBalance>)@event).Data;
                    _quikEventsInvoker.OnAccountBalance(accountBalance);
                    break;

                case EventName.AccountPosition:
                    Trace.Assert(@event is Event<AccountPosition>);
                    var accPos = ((Event<AccountPosition>)@event).Data;
                    _quikEventsInvoker.OnAccountPosition(accPos);
                    break;

                case EventName.AllTrade:
                    Trace.Assert(@event is Event<AllTrade>);
                    var allTrade = ((Event<AllTrade>)@event).Data;
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
                    var dLimit = ((Event<DepoLimitEx>)@event).Data;
                    _quikEventsInvoker.OnDepoLimit(dLimit);
                    break;

                case EventName.DepoLimitDelete:
                    Trace.Assert(@event is Event<DepoLimitDelete>);
                    var dLimitDel = ((Event<DepoLimitDelete>)@event).Data;
                    _quikEventsInvoker.OnDepoLimitDelete(dLimitDel);
                    break;

                case EventName.Disconnected:
                    Trace.Assert(@event is Event<string>);
                    _quikEventsInvoker.OnDisconnected();
                    break;

                case EventName.Firm:
                    Trace.Assert(@event is Event<Firm>);
                    var frm = ((Event<Firm>)@event).Data;
                    _quikEventsInvoker.OnFirm(frm);
                    break;

                case EventName.FuturesClientHolding:
                    Trace.Assert(@event is Event<FuturesClientHolding>);
                    var futPos = ((Event<FuturesClientHolding>)@event).Data;
                    _quikEventsInvoker.OnFuturesClientHolding(futPos);
                    break;

                case EventName.FuturesLimitChange:
                    Trace.Assert(@event is Event<FuturesLimits>);
                    var futLimit = ((Event<FuturesLimits>)@event).Data;
                    _quikEventsInvoker.OnFuturesLimitChange(futLimit);
                    break;

                case EventName.FuturesLimitDelete:
                    Trace.Assert(@event is Event<FuturesLimitDelete>);
                    var limDel = ((Event<FuturesLimitDelete>)@event).Data;
                    _quikEventsInvoker.OnFuturesLimitDelete(limDel);
                    break;

                case EventName.Init:
                    // Этот callback никогда не будет вызван так как на момент получения вызова OnInit в lua скрипте
                    // соединение с библиотекой QuikSharp не будет еще установлено. То есть этот callback не имеет смысла.
                    break;

                case EventName.MoneyLimit:
                    Trace.Assert(@event is Event<MoneyLimitEx>);
                    var mLimit = ((Event<MoneyLimitEx>)@event).Data;
                    _quikEventsInvoker.OnMoneyLimit(mLimit);
                    break;

                case EventName.MoneyLimitDelete:
                    Trace.Assert(@event is Event<MoneyLimitDelete>);
                    var mLimitDel = ((Event<MoneyLimitDelete>)@event).Data;
                    _quikEventsInvoker.OnMoneyLimitDelete(mLimitDel);
                    break;

                case EventName.NegDeal:
                    break;

                case EventName.NegTrade:
                    break;

                case EventName.Order:
                    Trace.Assert(@event is Event<Order>);
                    var ord = ((Event<Order>)@event).Data;
                    ord.LuaTimeStamp = @event.CreatedTime;
                    _quikEventsInvoker.OnOrder(ord);
                    break;

                case EventName.Param:
                    Trace.Assert(@event is Event<Param>);
                    var data = ((Event<Param>)@event).Data;
                    _quikEventsInvoker.OnParam(data);
                    break;

                case EventName.Quote:
                    Trace.Assert(@event is Event<OrderBook>);
                    var ob = ((Event<OrderBook>)@event).Data;
                    ob.LuaTimeStamp = @event.CreatedTime;
                    _quikEventsInvoker.OnQuote(ob);
                    break;

                case EventName.Stop:
                    Trace.Assert(@event is Event<string>);
                    _quikEventsInvoker.OnStop(int.Parse(((Event<string>)@event).Data));
                    break;

                case EventName.StopOrder:
                    Trace.Assert(@event is Event<StopOrder>);
                    StopOrder stopOrder = ((Event<StopOrder>)@event).Data;
                    _quikEventsInvoker.OnStopOrder(stopOrder);
                    break;

                case EventName.Trade:
                    Trace.Assert(@event is Event<Trade>);
                    var trade = ((Event<Trade>)@event).Data;
                    trade.LuaTimeStamp = @event.CreatedTime;
                    _quikEventsInvoker.OnTrade(trade);
                    break;

                case EventName.TransReply:
                    Trace.Assert(@event is Event<TransactionReply>);
                    var trReply = ((Event<TransactionReply>)@event).Data;
                    trReply.LuaTimeStamp = @event.CreatedTime;
                    _quikEventsInvoker.OnTransReply(trReply);
                    break;

                case EventName.Candle:
                    Trace.Assert(@event is Event<Candle>);
                    var candle = ((Event<Candle>)@event).Data;
                    _quikEventsInvoker.OnCandle(candle);
                    break;

                case EventName.Error:
                    Trace.Assert(@event is Event<string>);
                    var error = ((Event<string>)@event).Data;
                    Trace.TraceError(error);
                    _quikEventsInvoker.OnError(error);
                    break;

                default:
                    throw new EventTypeNotSupportedException($"Тип события: '{@event.Name}' не поддерживается.");
            }
        }
    }
}
