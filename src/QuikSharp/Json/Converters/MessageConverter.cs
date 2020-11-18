using Newtonsoft.Json.Linq;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Json.Converters
{
    public class MessageConverter : JsonCreationConverter<IMessage>
    {
        private QuikClient.QuikClient _service; // TODO: Убрать использование.

        public MessageConverter(QuikClient.QuikClient service)
        {
            _service = service;
        }

        // we learn object type from correlation id and a type stored in responses dictionary
        // ReSharper disable once RedundantAssignment
        protected override IMessage Create(Type objectType, JObject jObject)
        {
            if (FieldExists("Error", jObject))
            {
                var id = jObject.GetValue("id").Value<long>();
                var cmd = jObject.GetValue("cmd").Value<string>();
                var message = jObject.GetValue("Error").Value<string>();
                LuaException exception;
                switch (cmd)
                {
                    case "lua_transaction_error":
                        exception = new TransactionException(message);
                        break;

                    default:
                        exception = new LuaException(message);
                        break;
                }

                _service.PendingResponses.TryRemove(id, out var pendingResponse);
                pendingResponse.TaskCompletionSource.SetException(exception);
                // terminate listener task that was processing this task
                throw exception;
            }
            else if (FieldExists("id", jObject))
            {
                // Если есть id, значит пришел ответ на запрос (IRespose).
                var id = jObject.GetValue("id").Value<long>();
                objectType = _service.PendingResponses[id].ResultType;
                return (IResult)Activator.CreateInstance(objectType);
            }
            else if (FieldExists("cmd", jObject))
            {
                // without id we have an event
                EventName eventName;
                string cmd = jObject.GetValue("cmd").Value<string>();
                var parsed = Enum.TryParse(cmd, true, out eventName);
                if (parsed)
                {
                    switch (eventName)
                    {
                        case EventName.AccountBalance:
                            return new Event<AccountBalance> { Data = new AccountBalance() };

                        case EventName.AccountPosition:
                            return new Event<AccountPosition> { Data = new AccountPosition() };

                        case EventName.AllTrade:
                            return new Event<AllTrade> { Data = new AllTrade() };

                        case EventName.CleanUp:
                        case EventName.Close:
                        case EventName.Connected:
                        case EventName.Disconnected:
                        case EventName.Init:

                        case EventName.Stop:
                            return new Event<string>();

                        case EventName.DepoLimit:
                            return new Event<DepoLimitEx> { Data = new DepoLimitEx() };

                        case EventName.DepoLimitDelete:
                            return new Event<DepoLimitDelete> { Data = new DepoLimitDelete() };

                        case EventName.Firm:
                            return new Event<Firm> { Data = new Firm() };

                        case EventName.FuturesClientHolding:
                            return new Event<FuturesClientHolding> { Data = new FuturesClientHolding() };

                        case EventName.FuturesLimitChange:
                            return new Event<FuturesLimits> { Data = new FuturesLimits() };

                        case EventName.FuturesLimitDelete:
                            return new Event<FuturesLimitDelete> { Data = new FuturesLimitDelete() };

                        case EventName.MoneyLimit:
                            return new Event<MoneyLimitEx> { Data = new MoneyLimitEx() };

                        case EventName.MoneyLimitDelete:
                            return new Event<MoneyLimitDelete> { Data = new MoneyLimitDelete() };

                        case EventName.NegDeal:
                            break;

                        case EventName.NegTrade:
                            break;

                        case EventName.Order:
                            return new Event<Order> { Data = new Order() };

                        case EventName.Param:
                            return new Event<Param> { Data = new Param() };

                        case EventName.Quote:
                            return new Event<OrderBook> { Data = new OrderBook() };

                        case EventName.StopOrder:
                            return new Event<StopOrder> { Data = new StopOrder() };

                        case EventName.Trade:
                            return new Event<Trade> { Data = new Trade() };

                        case EventName.TransReply:
                            return new Event<TransactionReply> { Data = new TransactionReply() };

                        case EventName.Candle:
                            return new Event<Candle> { Data = new Candle() };

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // if we have a custom event (e.g. add some processing  of standard Quik event) then we must process it here
                    switch (cmd)
                    {
                        case "Error":
                            return new Event<string>();

                        default:
                            //return (IMessage)Activator.CreateInstance(typeof(Message<string>));
                            throw new InvalidOperationException("Unknown command in a message: " + cmd);
                    }
                }
            }

            throw new ArgumentException("Bad message format: no cmd or Error fields");
        }

        private static bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
