using QuikSharp.DataStructures;
using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.StopOrders
{
    public interface IStopOrderFunctions
    {
        

        Task<long> CreateStopOrderAsync(StopOrder stopOrder);

        Task<long> KillStopOrderAsync(StopOrder stopOrder);
    }
}
