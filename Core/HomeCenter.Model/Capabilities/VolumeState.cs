﻿using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.ValueTypes;
using HomeCenter.Model.Commands.Device;

namespace HomeCenter.Model.Capabilities
{
    public class VolumeState : State
    {
        public static string StateName { get; } = nameof(VolumeState);

        public VolumeState(StringValue ReadWriteMode = default) : base(ReadWriteMode)
        {
            this[StateProperties.Value] = new DoubleValue();
            this[StateProperties.MaxValue] = new DoubleValue(100.0);
            this[StateProperties.MinValue] = new DoubleValue(0.0);
            this[StateProperties.StateName] = new StringValue(nameof(VolumeState));
            this[StateProperties.CapabilityName] = new StringValue(Constants.Capabilities.SpeakerController);
            this[StateProperties.SupportedCommands] = new StringListValue(nameof(VolumeUpCommand), nameof(VolumeDownCommand), nameof(VolumeSetCommand));
        }
    }
}