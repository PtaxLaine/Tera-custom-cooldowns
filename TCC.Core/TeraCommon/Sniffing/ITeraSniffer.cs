﻿// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TCC.TeraCommon.Game;

namespace TCC.TeraCommon.Sniffing
{
    public interface ITeraSniffer
    {
        bool Enabled { get; set; }
        event Action<Message> MessageReceived;
        event Action<Server> NewConnection;
    }
}