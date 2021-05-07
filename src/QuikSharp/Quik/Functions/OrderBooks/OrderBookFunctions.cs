// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;

namespace QuikSharp.Quik.Functions.OrderBooks
{
    /// <summary>
    /// Функции для работы со стаканом котировок.
    /// </summary>
    public class OrderBookFunctions : FunctionsBase, IOrderBookFunctions
    {
        public OrderBookFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }
        
        /// <inheritdoc/>
        public Task<bool> SubscribeAsync(ISecurity security)
        {
            return SubscribeAsync(security.ClassCode, security.SecCode);
        }

        /// <inheritdoc/>
        public Task<bool> SubscribeAsync(string class_code, string sec_code)
        {
            return ExecuteCommandAsync<bool>("Subscribe_Level_II_Quotes", new[] { class_code, sec_code });
        }

        /// <inheritdoc/>
        public Task<bool> UnsubscribeAsync(ISecurity security)
        {
            return UnsubscribeAsync(security.ClassCode, security.SecCode);
        }

        /// <inheritdoc/>
        public Task<bool> UnsubscribeAsync(string class_code, string sec_code)
        {
            return ExecuteCommandAsync<bool>("Unsubscribe_Level_II_Quotes", new[] { class_code, sec_code });
        }

        /// <inheritdoc/>
        public Task<bool> IsSubscribedAsync(ISecurity security)
        {
            return IsSubscribedAsync(security.ClassCode, security.SecCode);
        }

        /// <inheritdoc/>
        public Task<bool> IsSubscribedAsync(string class_code, string sec_code)
        {
            return ExecuteCommandAsync<bool>("IsSubscribed_Level_II_Quotes", new[] { class_code, sec_code });
        }

        /// <inheritdoc/>
        public Task<OrderBook> GetQuoteLevel2Async(string class_code, string sec_code)
        {
            return ExecuteCommandAsync<OrderBook>("GetQuoteLevel2", new[] { class_code, sec_code });
        }
    }
}