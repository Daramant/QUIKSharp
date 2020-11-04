using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikService
{
    public interface IQuikService
    {
        void Start();
        void Stop();

        Task<TResponse> Send<TResponse>(IRequest request, int timeout = 0)
            where TResponse : class, IResponse, new();
    }
}
