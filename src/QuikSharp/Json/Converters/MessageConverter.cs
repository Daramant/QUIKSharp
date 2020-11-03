using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Converters
{
    internal class MessageConverter : JsonCreationConverter<IMessage>
    {
        private QuikService _service;

        public MessageConverter(QuikService service)
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
                LuaException exn;
                switch (cmd)
                {
                    case "lua_transaction_error":
                        exn = new TransactionException(message);
                        break;

                    default:
                        exn = new LuaException(message);
                        break;
                }

                KeyValuePair<TaskCompletionSource<IMessage>, Type> kvp;
                _service.Responses.TryRemove(id, out kvp);
                var tcs = kvp.Key;
                tcs.SetException(exn);
                // terminate listener task that was processing this task
                throw exn;
            }
            else if (FieldExists("id", jObject))
            {
                var id = jObject.GetValue("id").Value<long>();
                objectType = _service.Responses[id].Value;
                return (IMessage)Activator.CreateInstance(objectType);
            }
            else if (FieldExists("cmd", jObject))
            {
                // without id we have an event
                EventNames eventName;
                string cmd = jObject.GetValue("cmd").Value<string>();
                var parsed = Enum.TryParse(cmd, true, out eventName);
                if (parsed)
                {
                    switch (eventName)
                    {
                        case EventNames.OnAccountBalance:
                            return new Message<AccountBalance> { Data = new AccountBalance() };

                        case EventNames.OnAccountPosition:
                            return new Message<AccountPosition> { Data = new AccountPosition() };

                        case EventNames.OnAllTrade:
                            return new Message<AllTrade> { Data = new AllTrade() };

                        case EventNames.OnCleanUp:
                        case EventNames.OnClose:
                        case EventNames.OnConnected:
                        case EventNames.OnDisconnected:
                        case EventNames.OnInit:

                        case EventNames.OnStop:
                            return new Message<string>();

                        case EventNames.OnDepoLimit:
                            return new Message<DepoLimitEx> { Data = new DepoLimitEx() };

                        case EventNames.OnDepoLimitDelete:
                            return new Message<DepoLimitDelete> { Data = new DepoLimitDelete() };

                        case EventNames.OnFirm:
                            return new Message<Firm> { Data = new Firm() };

                        case EventNames.OnFuturesClientHolding:
                            return new Message<FuturesClientHolding> { Data = new FuturesClientHolding() };

                        case EventNames.OnFuturesLimitChange:
                            return new Message<FuturesLimits> { Data = new FuturesLimits() };

                        case EventNames.OnFuturesLimitDelete:
                            return new Message<FuturesLimitDelete> { Data = new FuturesLimitDelete() };

                        case EventNames.OnMoneyLimit:
                            return new Message<MoneyLimitEx> { Data = new MoneyLimitEx() };

                        case EventNames.OnMoneyLimitDelete:
                            return new Message<MoneyLimitDelete> { Data = new MoneyLimitDelete() };

                        case EventNames.OnNegDeal:
                            break;

                        case EventNames.OnNegTrade:
                            break;

                        case EventNames.OnOrder:
                            return new Message<Order> { Data = new Order() };

                        case EventNames.OnParam:
                            return new Message<Param> { Data = new Param() };

                        case EventNames.OnQuote:
                            return new Message<OrderBook> { Data = new OrderBook() };

                        case EventNames.OnStopOrder:
                            return new Message<StopOrder> { Data = new StopOrder() };

                        case EventNames.OnTrade:
                            return new Message<Trade> { Data = new Trade() };

                        case EventNames.OnTransReply:
                            return new Message<TransactionReply> { Data = new TransactionReply() };

                        case EventNames.NewCandle:
                            return new Message<Candle> { Data = new Candle() };

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
                            return new Message<string>();

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
