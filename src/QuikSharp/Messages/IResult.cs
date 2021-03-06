﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IResult : IMessage
    {
    }

    public interface IResult<T> : IMessage<T>, IResult
    {
    }
}
