using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikFunctions.Candles;
using QuikSharp.QuikFunctions.OrderBooks;
using QuikSharp.QuikFunctions.Orders;
using QuikSharp.QuikFunctions.Services;
using QuikSharp.QuikFunctions.StopOrders;
using QuikSharp.QuikFunctions.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikClient
{
    public interface IQuikEventHandler
    {
        void Handle(IEvent @event);
    }
}
