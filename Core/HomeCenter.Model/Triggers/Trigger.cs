﻿using HomeCenter.Model.Conditions;
using HomeCenter.Model.Core;
using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Events;
using Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Model.Triggers
{
    public class Trigger
    {
        [Map] public Event Event { get; private set; }
        [Map] public IList<Command> Commands { get; set; }
        [Map] public Schedule Schedule { get; private set; }
        [Map] public ConditionContainer Condition { get; private set; }

        public Task<bool> ValidateCondition() => Condition?.Validate() ?? Task.FromResult(true);

        public TriggerJobDataDTO ToJobData(PID actor) => new TriggerJobDataDTO
        {
            Condition = Condition,
            Actor = actor,
            //TODO DNF
            //Command = Command
        };

        public TriggerJobDataDTO ToJobDataWithFinish(PID actor) => new TriggerJobDataDTO
        {
            Condition = Condition,
            Actor = actor,
            // TODO DNF
            //Command = Command,
           // FinishCommand = FinishCommand,
            FinishCommandTime = Schedule.WorkingTime
        };
    }
}