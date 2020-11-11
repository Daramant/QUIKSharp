// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikClient;

namespace QuikSharp.QuikFunctions.OrderBooks
{
    /// <summary>
    /// Функции для работы со стаканом котировок
    /// </summary>
    public class OrderBookFunctions : IOrderBookFunctions
    {
        private readonly IQuikClient _quikClient;

        public OrderBookFunctions(IQuikClient quikClient)
        {
            _quikClient = quikClient;
        }
        
        public async Task<bool> SubscribeAsync(ISecurity security)
        {
            return await SubscribeAsync(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> SubscribeAsync(string class_code, string sec_code)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string>(class_code + "|" + sec_code, "Subscribe_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> UnsubscribeAsync(ISecurity security)
        {
            return await UnsubscribeAsync(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> UnsubscribeAsync(string class_code, string sec_code)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string>(class_code + "|" + sec_code, "Unsubscribe_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsSubscribedAsync(ISecurity security)
        {
            return await IsSubscribedAsync(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> IsSubscribedAsync(string class_code, string sec_code)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string>(class_code + "|" + sec_code, "IsSubscribed_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<OrderBook> GetQuoteLevel2Async(string class_code, string sec_code)
        {
            var response = await _quikClient.SendAsync<Result<OrderBook>>(
                (new Command<string>(class_code + "|" + sec_code, "GetQuoteLevel2"))).ConfigureAwait(false);
            return response.Data;
        }
    }
}