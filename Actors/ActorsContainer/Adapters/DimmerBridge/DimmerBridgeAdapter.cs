﻿using HomeCenter.CodeGeneration;
using HomeCenter.Model.Adapters;
using HomeCenter.Model.Capabilities;
using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Commands.Service;
using HomeCenter.Model.Messages.Events.Device;
using HomeCenter.Model.Messages.Queries.Device;
using HomeCenter.Model.Messages.Queries.Service;
using Microsoft.Extensions.Logging;
using Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Adapters.CurrentBridge
{
    [ProxyCodeGenerator]
    public abstract class DimmerBridgeAdapter : Adapter
    {
        private const int I2C_ACTION_DIMMER = 5;
        private readonly Dictionary<int, double> _state = new Dictionary<int, double>();
        private int _i2cAddress;

        protected DimmerBridgeAdapter()
        {
            _requierdProperties.Add(MessageProperties.PinNumber);
        }

        protected override async Task OnStarted(IContext context)
        {
            await base.OnStarted(context);

            _i2cAddress = AsInt(MessageProperties.Address);

            var registration = new RegisterSerialCommand(Self, I2C_ACTION_DIMMER, new Format[]
            {
                new Format(1, typeof(byte), MessageProperties.PinNumber),
                new Format(2, typeof(float), MessageProperties.Value)
            });

            await MessageBroker.SendToService(registration);
        }

        protected async Task Handle(SerialResultEvent serialResult)
        {
            var pin = serialResult.AsByte(MessageProperties.PinNumber);
            var current = serialResult.AsDouble(MessageProperties.Value);

            if (_state.ContainsKey(pin))
            {
                var oldValue = _state[pin];

                _state[pin] = await UpdateState(CurrentState.StateName, oldValue, current, new Dictionary<string, string>() { [MessageProperties.PinNumber] = pin.ToString() });
            }
        }

        protected DiscoveryResponse Discover(DiscoverQuery message)
        {
            RegisterPinNumber(message);

            return new DiscoveryResponse(RequierdProperties(), new CurrentState());
        }

        private void RegisterPinNumber(DiscoverQuery message)
        {
            var pin = message.AsByte(MessageProperties.PinNumber);
            var registrationMessage = new byte[] { I2C_ACTION_DIMMER, pin };

            if (!_state.ContainsKey(pin))
            {
                _state.Add(pin, 0);
            }

            MessageBroker.SendToService(I2cCommand.Create(_i2cAddress, registrationMessage));
        }
    }
}