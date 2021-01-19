using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikEvents
{
    public class EventTypeProvider : IEventTypeProvider
    {
        public bool TryGetEventType(EventName eventName, out Type eventType)
        {
            eventType = null;

            switch (eventName)
            {
                case EventName.AccountBalance:
                    eventType = typeof(Event<AccountBalance>);
                    break;

                case EventName.AccountPosition:
                    eventType = typeof(Event<AccountPosition>);
                    break;

                case EventName.AllTrade:
                    eventType = typeof(Event<AllTrade>);
                    break;

                case EventName.CleanUp:
                case EventName.Close:
                case EventName.Connected:
                case EventName.Disconnected:
                case EventName.Init:

                case EventName.Stop:
                    eventType = typeof(Event<string>);
                    break;
                case EventName.DepoLimit:
                    eventType = typeof(Event<DepoLimitEx>);
                    break;
                case EventName.DepoLimitDelete:
                    eventType = typeof(Event<DepoLimitDelete>);
                    break;
                case EventName.Firm:
                    eventType = typeof(Event<Firm>);
                    break;
                case EventName.FuturesClientHolding:
                    eventType = typeof(Event<FuturesClientHolding>);
                    break;
                case EventName.FuturesLimitChange:
                    eventType = typeof(Event<FuturesLimits>);
                    break;
                case EventName.FuturesLimitDelete:
                    eventType = typeof(Event<FuturesLimitDelete>);
                    break;
                case EventName.MoneyLimit:
                    eventType = typeof(Event<MoneyLimitEx>);
                    break;
                case EventName.MoneyLimitDelete:
                    eventType = typeof(Event<MoneyLimitDelete>);
                    break;
                case EventName.NegDeal:
                    break;

                case EventName.NegTrade:
                    break;

                case EventName.Order:
                    eventType = typeof(Event<Order>);
                    break;
                case EventName.Param:
                    eventType = typeof(Event<Param>);
                    break;
                case EventName.Quote:
                    eventType = typeof(Event<OrderBook>);
                    break;
                case EventName.StopOrder:
                    eventType = typeof(Event<StopOrder>);
                    break;
                case EventName.Trade:
                    eventType = typeof(Event<Trade>);
                    break;
                case EventName.TransReply:
                    eventType = typeof(Event<TransactionReply>);
                    break;
                case EventName.Candle:
                    eventType = typeof(Event<Candle>);
                    break;
            }

            return eventType != null;
        }
    }
}
