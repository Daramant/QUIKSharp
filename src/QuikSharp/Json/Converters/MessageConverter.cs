using Newtonsoft.Json.Linq;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Messages;
using QuikSharp.QuikService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Json.Converters
{
    public class MessageConverter : JsonCreationConverter<IMessage>
    {
        private QuikService.QuikService _service; // TODO: Убрать использование.

        public MessageConverter(QuikService.QuikService service)
        {
            _service = service;
        }

        // we learn object type from correlation id and a type stored in responses dictionary
        // ReSharper disable once RedundantAssignment
        protected override IMessage Create(Type objectType, JObject jObject)
        {
            if (FieldExists("lua_error", jObject))
            {
                var id = jObject.GetValue("id").Value<long>();
                var cmd = jObject.GetValue("cmd").Value<string>();
                var message = jObject.GetValue("lua_error").Value<string>();
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
                objectType = _service.PendingResponses[id].ResponseType;
                return (IResponse)Activator.CreateInstance(objectType);
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
                            return new Response<AccountBalance> { Data = new AccountBalance() };

                        case EventName.AccountPosition:
                            return new Response<AccountPosition> { Data = new AccountPosition() };

                        case EventName.AllTrade:
                            return new Response<AllTrade> { Data = new AllTrade() };

                        case EventName.CleanUp:
                        case EventName.Close:
                        case EventName.Connected:
                        case EventName.Disconnected:
                        case EventName.Init:

                        case EventName.Stop:
                            return new Response<string>();

                        case EventName.DepoLimit:
                            return new Response<DepoLimitEx> { Data = new DepoLimitEx() };

                        case EventName.DepoLimitDelete:
                            return new Response<DepoLimitDelete> { Data = new DepoLimitDelete() };

                        case EventName.Firm:
                            return new Response<Firm> { Data = new Firm() };

                        case EventName.FuturesClientHolding:
                            return new Response<FuturesClientHolding> { Data = new FuturesClientHolding() };

                        case EventName.FuturesLimitChange:
                            return new Response<FuturesLimits> { Data = new FuturesLimits() };

                        case EventName.FuturesLimitDelete:
                            return new Response<FuturesLimitDelete> { Data = new FuturesLimitDelete() };

                        case EventName.MoneyLimit:
                            return new Response<MoneyLimitEx> { Data = new MoneyLimitEx() };

                        case EventName.MoneyLimitDelete:
                            return new Response<MoneyLimitDelete> { Data = new MoneyLimitDelete() };

                        case EventName.NegDeal:
                            break;

                        case EventName.NegTrade:
                            break;

                        case EventName.Order:
                            return new Response<Order> { Data = new Order() };

                        case EventName.Param:
                            return new Response<Param> { Data = new Param() };

                        case EventName.Quote:
                            return new Response<OrderBook> { Data = new OrderBook() };

                        case EventName.StopOrder:
                            return new Response<StopOrder> { Data = new StopOrder() };

                        case EventName.Trade:
                            return new Response<Trade> { Data = new Trade() };

                        case EventName.TransReply:
                            return new Response<TransactionReply> { Data = new TransactionReply() };

                        case EventName.NewCandle:
                            return new Response<Candle> { Data = new Candle() };

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // if we have a custom event (e.g. add some processing  of standard Quik event) then we must process it here
                    switch (cmd)
                    {
                        case "lua_error":
                            return new Response<string>();

                        default:
                            //return (IMessage)Activator.CreateInstance(typeof(Message<string>));
                            throw new InvalidOperationException("Unknown command in a message: " + cmd);
                    }
                }
            }

            throw new ArgumentException("Bad message format: no cmd or lua_error fields");
        }

        private static bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
