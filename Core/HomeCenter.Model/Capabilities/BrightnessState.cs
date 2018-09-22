﻿using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.ValueTypes;

namespace HomeCenter.Model.Capabilities
{
    public class BrightnessState : State
    {
        public BrightnessState(StringValue ReadWriteMode = default) : base(ReadWriteMode)
        {
            //StateName = nameof(BrightnessState);
            //Capability = Constants.Capabilities.BrightnessController;

            this[StateProperties.Value] = new DoubleValue();
            this[StateProperties.MaxValue] = new DoubleValue(100.0);
            this[StateProperties.MinValue] = new DoubleValue(0.0);
            this[StateProperties.ValueUnit] = (StringValue)ValueUnit.Percent;
        }
    }
}
