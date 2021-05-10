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
    /// Функции для заказа стакана котировок.
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
        public Task<bool> SubscribeAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<bool>("Subscribe_Level_II_Quotes", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<bool> UnsubscribeAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<bool>("Unsubscribe_Level_II_Quotes", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<bool> IsSubscribedAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<bool>("IsSubscribed_Level_II_Quotes", new[] { classCode, secCode });
        }
    }
}