using QuikSharp.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public class MessageFactory : IMessageFactory
    {
        private readonly IIdProvider _idProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MessageFactory(
            IIdProvider idProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _idProvider = idProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public ICommand<T> CreateCommand<T>(string name, T data = default, DateTime? validUntil = null)
        {
            var command = new Command<T>(name, data, validUntil);

            command.Id = _idProvider.GetUniqueCommandId();
            command.CreatedTime = _dateTimeProvider.NowInMilliseconds;

            return command;
        }
    }
}
