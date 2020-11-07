using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikService
{
    public struct PendingResponse
    {
        public ICommand Request { get; set; }

        public Type ResponseType { get; set; }

        public TaskCompletionSource<IResult> TaskCompletionSource { get; set; }

        public PendingResponse(ICommand request, Type responseType, TaskCompletionSource<IResult> taskCompletionSource)
        {
            Request = request;
            ResponseType = responseType;
            TaskCompletionSource = taskCompletionSource;
        }
    }
}
