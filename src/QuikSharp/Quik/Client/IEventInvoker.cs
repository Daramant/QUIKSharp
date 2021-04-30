using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.Orders;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.Quik.Functions.StopOrders;
using QuikSharp.Quik.Functions.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Client
{
    public interface IEventInvoker
    {
        void Invoke(IEvent @event);
    }
}
