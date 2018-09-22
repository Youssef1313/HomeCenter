﻿using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.ValueTypes;

namespace HomeCenter.Model.Capabilities
{
    public class HumidityState : State
    {
        public static string StateName { get; } = nameof(HumidityState);

        public HumidityState(StringValue ReadWriteMode = default) : base(ReadWriteMode)
        {
            this[StateProperties.StateName] = new StringValue(nameof(HumidityState));
            this[StateProperties.CapabilityName] = new StringValue(Constants.Capabilities.TemperatureController);
            this[StateProperties.Value] = new DoubleValue();
            //this[StateProperties.SupportedCommands] = new StringListValue(CommandType.TurnOnCommand, CommandType.TurnOffCommand);
        }
    }
}