using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public class Result<T>: Message<T>, IResult<T>
    {
        public Result()
            : base()
        {
        }

        public Result(T data)
            : base(data)
        {
        }
    }
}
