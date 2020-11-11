// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.QuikClient;
using QuikSharp.QuikEvents;
using QuikSharp.QuikFunctions;
using System;

namespace QuikSharp.Quik
{
    /// <summary>
    /// Quik interface in .NET
    /// </summary>
    public sealed class Quik : IQuik
    {
        /// <inheritdoc />
        public IQuikClient Client { get; }

        /// <inheritdoc />
        public IQuikFunctions Functions { get; }

        /// <inheritdoc />
        public IQuikEvents Events { get; }

        /// <summary>
        /// Quik interface in .NET constructor
        /// </summary>
        public Quik(
            IQuikClient quickClient, 
            IQuikFunctions quikFunctions,
            IQuikEvents quikEvents)
        {
            Client = quickClient;
            Functions = quikFunctions;
            Events = quikEvents;
        }
    }
}