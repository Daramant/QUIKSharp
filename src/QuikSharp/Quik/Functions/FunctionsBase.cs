using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FunctionsBase
    {
        protected IMessageFactory MessageFactory { get; }

        protected IQuikClient QuikClient { get; }

        protected ITypeConverter TypeConverter { get; }

        public FunctionsBase(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
        {
            MessageFactory = messageFactory;
            QuikClient = quikClient;
            TypeConverter = typeConverter;
        }

        protected Task<TResult> ExecuteCommandAsync<TResult>(string name, string[] data = null, TimeSpan? timeout = null)
        {
            return ExecuteCommandAsync<string[], TResult>(name, data, timeout);
        }

        protected async Task<TResult> ExecuteCommandAsync<TData, TResult>(string name, TData data = default, TimeSpan? timeout = null)
        {
            var command = MessageFactory.CreateCommand(name, data);
            var response = await QuikClient.SendAsync<IResult<TResult>>(command, timeout).ConfigureAwait(false);
            return response.Data;
        }
    }
}
