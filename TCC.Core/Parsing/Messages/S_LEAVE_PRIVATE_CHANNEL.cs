﻿using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LEAVE_PRIVATE_CHANNEL : ParsedMessage
    {
        public uint Id { get; private set; }
        public S_LEAVE_PRIVATE_CHANNEL(TeraMessageReader reader) : base(reader)
        {
            Id = reader.ReadUInt32();
        }
    }
}
