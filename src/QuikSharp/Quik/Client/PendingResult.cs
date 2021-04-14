using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikClient
{
    public struct PendingResult
    {
        public ICommand Command { get; set; }

        public Type ResultType { get; set; }

        public TaskCompletionSource<IResult> TaskCompletionSource { get; set; }

        public PendingResult(ICommand command, Type resultType, TaskCompletionSource<IResult> taskCompletionSource)
        {
            Command = command;
            ResultType = resultType;
            TaskCompletionSource = taskCompletionSource;
        }
    }
}
