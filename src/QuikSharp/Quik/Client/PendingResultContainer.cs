using QuikSharp.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuikSharp.Quik.Client
{
    public class PendingResultContainer : IPendingResultContainer, IResultTypeProvider
    {
        private readonly ConcurrentDictionary<long, PendingResult> _pendingResults = new ConcurrentDictionary<long, PendingResult>();

        /// <inheritdoc/>
        public void Add(long commandId, PendingResult pendingResult)
        {
            _pendingResults[commandId] = pendingResult;
        }

        /// <inheritdoc/>
        public void Remove(long commandId)
        {
            _pendingResults.TryRemove(commandId, out var _);
        }

        /// <inheritdoc/>
        public bool TryRemove(long commandId, out PendingResult pendingResult)
        {
            return _pendingResults.TryRemove(commandId, out pendingResult);
        }

        /// <inheritdoc/>
        public bool TryGet(long commandId, out PendingResult pendingResult)
        {
            return _pendingResults.TryGetValue(commandId, out pendingResult);
        }

        /// <inheritdoc/>
        public void CancelAll()
        {
            // cancel responses to release waiters
            foreach (var responseKey in _pendingResults.Keys.ToList())
            {
                if (_pendingResults.TryRemove(responseKey, out var pendingResponse))
                {
                    pendingResponse.TaskCompletionSource.TrySetCanceled();
                }
            }
        }

        /// <inheritdoc/>
        public bool TryGetResultType(long commandId, out Type resultType)
        {
            if (_pendingResults.TryGetValue(commandId, out var pendingResult))
            {
                resultType = pendingResult.ResultType;
                return true;
            }

            resultType = null;
            return false;
        }
    }
}
