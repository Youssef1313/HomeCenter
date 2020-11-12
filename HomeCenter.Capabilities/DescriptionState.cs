﻿using HomeCenter.Abstractions;
using HomeCenter.Abstractions.Defaults;
using HomeCenter.Messages.Queries.Device;

namespace HomeCenter.Capabilities
{
    public class DescriptionState : StateBase
    {
        public static string StateName { get; } = nameof(DescriptionState);

        public DescriptionState(string ReadWriteMode = default) : base(ReadWriteMode)
        {
            this[StateProperties.StateName] = nameof(DescriptionState);
            this[StateProperties.CapabilityName] = Abstractions.Defaults.Capabilities.InfoController;

            this.SetPropertyList(StateProperties.SupportedCommands, nameof(DescriptionQuery));
        }
    }
}