using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Serialization
{
    public interface IResultTypeProvider
    {
        bool TryGetResultType(long commandId, out Type resultType);
    }
}
