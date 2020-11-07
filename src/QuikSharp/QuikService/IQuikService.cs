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

        Task<TResult> SendAsync<TResult>(ICommand request, int timeout = 0)
            where TResult : class, IResult, new();

        int GetUniqueTransactionId();
    }
}
