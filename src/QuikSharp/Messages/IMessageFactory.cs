using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IMessageFactory
    {
        ICommand<T> CreateCommand<T>(string name, T data = default(T), DateTime? validUntil = null);
    }
}
