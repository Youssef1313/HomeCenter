﻿using HomeCenter.CodeGeneration;
using HomeCenter.Model.Adapters;
using HomeCenter.Model.Capabilities;
using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Commands.Device;
using HomeCenter.Model.Messages.Queries.Device;
using HomeCenter.Tests.Dummies;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace HomeCenter.Tests.Mocks
{
    /// <summary>
    /// Adapter that have to be controlled with required properties when sending commands
    /// </summary>
    [ProxyCodeGenerator]
    public abstract class RequiredPropertiesAdapter : Adapter
    {
        public Subject<Command> CommandRecieved { get; } = new Subject<Command>();

        public RequiredPropertiesAdapter()
        {
            _requierdProperties.Add(MessageProperties.PinNumber);
        }

        protected DiscoveryResponse Handle(DiscoverQuery discoverQuery)
        {
            return new DiscoveryResponse(new string[] { MessageProperties.PinNumber }, new PowerState());
        }

        public void Handle(TurnOnCommand turnOnCommand)
        {
            CommandRecieved.OnNext(turnOnCommand);
        }

        protected RequiredPropertiesAdapter Handle(GetAdapterQuery getAdapterQuery)
        {
            return this;
        }

        public async Task PropertyChanged<T>(string state, T oldValue, T newValue, int pinNumber = 1)
        {
            await UpdateState(state, oldValue, newValue, new Dictionary<string, string>
            {
                [MessageProperties.PinNumber] = pinNumber.ToString()
            });
        }
    }
}