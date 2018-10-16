﻿using HomeCenter.CodeGeneration;
using HomeCenter.Model.Adapters;
using HomeCenter.Model.Capabilities;
using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.Extensions;
using HomeCenter.Model.Messages.Commands.Service;
using HomeCenter.Model.Messages.Queries.Device;
using HomeCenter.Model.Messages.Queries.Service;
using HomeCenter.Model.ValueTypes;
using Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Adapters.TemperatureBridge
{
    [ProxyCodeGenerator]
    public abstract class TemperatureBridgeAdapter : Adapter
    {
        private readonly Dictionary<IntValue, DoubleValue> _state = new Dictionary<IntValue, DoubleValue>();

        protected TemperatureBridgeAdapter()
        {
            _requierdProperties.Add(AdapterProperties.PinNumber);
        }

        protected override async Task OnStarted(IContext context)
        {
            await base.OnStarted(context).ConfigureAwait(false);

            var _i2cAddress = this[AdapterProperties.I2cAddress].AsInt();

            foreach (var val in this[AdapterProperties.UsedPins].AsStringList())
            {
                _state.Add(IntValue.FromString(val), 0);
            }
            var registration = new SerialRegistrationQuery(Self, 1, new Format[]
            {
                new Format(1, typeof(byte), "Pin"),
                new Format(2, typeof(float), "Temperature")
            });
            //TODO Send
            //TODO count size??
        }

        protected void Handle(SerialResultCommand serialResultCommand)
        {
            // _state[pin] = await UpdateState(TemperatureState.StateName, pin, (DoubleValue)temperature).ConfigureAwait(false);
        }

        protected DiscoveryResponse Discover(DiscoverQuery message)
        {
            return new DiscoveryResponse(RequierdProperties(), new TemperatureState(ReadWriteModeValues.Read));
        }
    }
}