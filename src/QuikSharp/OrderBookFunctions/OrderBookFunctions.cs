// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;
using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuickService;

namespace QuikSharp.OrderBookFunctions
{
    /// <summary>
    /// Функции для работы со стаканом котировок
    /// </summary>
    public class OrderBookFunctions : IOrderBookFunctions
    {
        private readonly IQuikService _quikService;

        public OrderBookFunctions(IQuikService quikService)
        {
            _quikService = quikService;
        }

        public async Task<bool> Subscribe(ISecurity security)
        {
            return await Subscribe(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> Subscribe(string class_code, string sec_code)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(class_code + "|" + sec_code, "Subscribe_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> Unsubscribe(ISecurity security)
        {
            return await Unsubscribe(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> Unsubscribe(string class_code, string sec_code)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(class_code + "|" + sec_code, "Unsubscribe_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsSubscribed(ISecurity security)
        {
            return await IsSubscribed(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<bool> IsSubscribed(string class_code, string sec_code)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(class_code + "|" + sec_code, "IsSubscribed_Level_II_Quotes"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<OrderBook> GetQuoteLevel2(string class_code, string sec_code)
        {
            var response = await _quikService.Send<Message<OrderBook>>(
                (new Message<string>(class_code + "|" + sec_code, "GetQuoteLevel2"))).ConfigureAwait(false);
            return response.Data;
        }
    }
}