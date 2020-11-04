// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.QuikService;
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
        public IQuikService Service { get; }

        /// <inheritdoc />
        public IQuikFunctions Functions { get; }

        /// <inheritdoc />
        public IQuikEvents Events { get; }

        /// <summary>
        /// Quik interface in .NET constructor
        /// </summary>
        public Quik(
            IQuikService quickService, 
            IQuikFunctions quikFunctions,
            IQuikEvents quikEvents)
        {
            Service = quickService;
            Functions = quikFunctions;
            Events = quikEvents;
        }
    }
}