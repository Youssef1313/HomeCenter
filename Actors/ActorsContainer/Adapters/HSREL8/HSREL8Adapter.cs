﻿using HomeCenter.Adapters.Common;
using HomeCenter.CodeGeneration;
using HomeCenter.Model.Adapters;
using HomeCenter.Model.Capabilities;
using HomeCenter.Model.Extensions;
using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Commands.Device;
using HomeCenter.Model.Messages.Queries.Device;
using Proto;
using System;
using System.Threading.Tasks;

namespace HomeCenter.Adapters.HSREL8
{
    [ProxyCodeGenerator]
    public abstract class HSREL8Adapter : CCToolsBaseAdapter
    {
        protected override async Task OnStarted(IContext context)
        {
            await base.OnStarted(context).ConfigureAwait(false);

            await SetState(new byte[] { 0x00, 255 }, true).ConfigureAwait(false);

            await ScheduleDeviceRefresh<RefreshStateJob>(_poolInterval).ConfigureAwait(false);
        }

        protected DiscoveryResponse QueryCapabilities(DiscoverQuery message)
        {
            return new DiscoveryResponse(RequierdProperties(), new PowerState());
        }

        public Task TurnOn(TurnOnCommand message)
        {
            int pinNumber = GetPin(message);

            return SetPortState(pinNumber, true, true);
        }

        public Task TurnOff(TurnOffCommand message)
        {
            int pinNumber = GetPin(message);

            return SetPortState(pinNumber, false, true);
        }

        private int GetPin(Command message)
        {
            var pinNumber = message.AsInt(AdapterProperties.PinNumber);
            if (pinNumber < 0 || pinNumber > 15) throw new ArgumentOutOfRangeException(nameof(pinNumber));
            return pinNumber;
        }
    }
}