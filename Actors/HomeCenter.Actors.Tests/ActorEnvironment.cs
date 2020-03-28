﻿using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Queries;
using HomeCenter.Model.Messages.Queries.Services;
using Microsoft.Reactive.Testing;
using Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Services.MotionService.Tests
{
    internal class ActorEnvironment : IDisposable
    {
        private readonly TestScheduler _scheduler;
        private readonly FakeLogger<LightAutomationServiceProxy> _logger;
        private readonly ITestableObservable<MotionEnvelope> _motionEvents;
        private readonly Dictionary<string, FakeMotionLamp> _lamps;
        private readonly RootContext _context = new RootContext();
        private readonly PID _pid;

        public ActorEnvironment(TestScheduler Scheduler, ITestableObservable<MotionEnvelope> MotionEvents, Dictionary<string, FakeMotionLamp> Lamps,
            FakeLogger<LightAutomationServiceProxy> Logger, LightAutomationServiceProxy actor)
        {
            _scheduler = Scheduler;
            _motionEvents = MotionEvents;
            _logger = Logger;
            _lamps = Lamps;
            _pid = _context.SpawnNamed(Props.FromProducer(() => actor), "motionService");
        }

        public void Send(Command actorMessage)
        {
            _context.Send(_pid, actorMessage);
            IsAlive();
        }

        public Task<R> Query<R>(Query actorMessage)
        {
            return _context.RequestAsync<R>(_pid, actorMessage);
        }

        /// <summary>
        /// Allows to wait for actor to process previous task
        /// </summary>
        public bool IsAlive()
        {
            return _context.RequestAsync<bool>(new PID("nonhost", "motionService"), IsAliveQuery.Default).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _logger.Dispose();
            _pid.StopAsync();
        }

        public void AdvanceToEnd() => _scheduler.AdvanceToEnd(_motionEvents);

        public void AdvanceTo(TimeSpan time) => _scheduler.AdvanceTo(time);

        /// <summary>
        /// We calculate next full second after given time and return just after this time (default 100ms)
        /// </summary>
        /// <param name="time"></param>
        public void AdvanceJustAfterRoundUp(TimeSpan time) => _scheduler.AdvanceJustAfter(TimeSpan.FromSeconds(Math.Ceiling(time.TotalSeconds)));

        public void AdvanceTo(long ticks) => _scheduler.AdvanceTo(ticks);

        public void AdvanceJustAfterEnd(int timeAfter = 500) => _scheduler.AdvanceJustAfterEnd(_motionEvents, timeAfter);

        public void AdvanceJustAfter(TimeSpan time) => _scheduler.AdvanceJustAfter(time);

        public void AdvanceJustBefore(TimeSpan time) => _scheduler.AdvanceJustBefore(time);

        public bool LampState(string lamp) => _lamps[lamp].IsTurnedOn;

        public void SendCommand(Command command) => Send(command);

        public void RunAfterFirstMove(Action<MotionEnvelope> action) => _motionEvents.Subscribe(action);

        public TimeSpan GetMotionTime(int vectorIndex) => TimeSpan.FromTicks(_motionEvents.Messages[vectorIndex].Time);

        public TimeSpan GetLastMotionTime() => TimeSpan.FromTicks(_motionEvents.Messages[_motionEvents.Messages.Count - 1].Time);
    }
}