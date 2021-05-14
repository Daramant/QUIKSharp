// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;

namespace QuikSharp.Transactions.PersistentStorages
{
    /// <summary>
    ///
    /// </summary>
    public interface IPersistentStorage
    {
        /// <summary>
        ///
        /// </summary>
        void Set<T>(string key, T value);

        /// <summary>
        ///
        /// </summary>
        T Get<T>(string key);

        /// <summary>
        ///
        /// </summary>
        bool Contains(string key);

        /// <summary>
        ///
        /// </summary>
        bool Remove(string key);
    }
}