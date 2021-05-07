using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Client
{
    public interface IPendingResultContainer
    {
        void Add(long commandId, PendingResult pendingResult);

        void Remove(long commandId);

        bool TryRemove(long commandId, out PendingResult pendingResult);

        bool TryGet(long commandId, out PendingResult pendingResult);

        void CancelAll();
    }
}
