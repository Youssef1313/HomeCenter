﻿using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.Messages.Commands.Device;
using HomeCenter.Model.ValueTypes;

namespace HomeCenter.Model.Capabilities
{
    public class SurroundSoundState : State
    {
        public static string StateName { get; } = nameof(SurroundSoundState);

        public SurroundSoundState(StringValue ReadWriteMode = default) : base(ReadWriteMode)
        {
            this[StateProperties.Value] = new StringValue();
            this[StateProperties.StateName] = new StringValue(nameof(SurroundSoundState));
            this[StateProperties.CapabilityName] = new StringValue(Constants.Capabilities.SpeakerController);
            this[StateProperties.SupportedCommands] = new StringListValue(nameof(ModeSetCommand));
        }
    }
}