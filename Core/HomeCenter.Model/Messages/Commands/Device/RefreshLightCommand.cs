﻿namespace HomeCenter.Model.Messages.Commands.Device
{
    public class RefreshLightCommand : Command
    {
        public static RefreshLightCommand Default = new RefreshLightCommand();

        public RefreshLightCommand()
        {
            LogLevel = nameof(Microsoft.Extensions.Logging.LogLevel.Trace);
        }
    }
}