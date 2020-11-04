using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikService
{
    public struct PendingResponse
    {
        public IRequest Request { get; set; }

        public Type ResponseType { get; set; }

        public TaskCompletionSource<IResponse> TaskCompletionSource { get; set; }

        public PendingResponse(IRequest request, Type responseType, TaskCompletionSource<IResponse> taskCompletionSource)
        {
            Request = request;
            ResponseType = responseType;
            TaskCompletionSource = taskCompletionSource;
        }
    }
}
