﻿using AutoMapper;
using HomeCenter.Model.Actors;
using HomeCenter.Model.Core;
using HomeCenter.Model.Messages.Events.Device;
using HomeCenter.Services.Configuration.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Moq;
using Proto;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Linq;

namespace HomeCenter.Services.MotionService.Tests
{
    internal class LightAutomationEnviromentBuilder
    {
        private int? _timeDuration;
        private TimeSpan? _periodicCheckTime;
        private ServiceDTO _serviceConfig;
        private TestScheduler _scheduler = new TestScheduler();
        private Container _container = new Container();

        private readonly ActorContext _actorContext;
        private readonly List<Recorded<Notification<MotionEnvelope>>> _motionEvents = new List<Recorded<Notification<MotionEnvelope>>>();
        private readonly List<Recorded<Notification<PowerStateChangeEvent>>> _lampEvents = new List<Recorded<Notification<PowerStateChangeEvent>>>();

        public LightAutomationEnviromentBuilder(ActorContext actorContext)
        {
            _actorContext = actorContext;
        }

        public LightAutomationEnviromentBuilder WithMotion(params Recorded<Notification<MotionEnvelope>>[] messages)
        {
            _motionEvents.AddRange(messages);
            return this;
        }

        public LightAutomationEnviromentBuilder WithMotions(Dictionary<int, string> motions)
        {
            _motionEvents.AddRange(motions.Select(x => new Recorded<Notification<MotionEnvelope>>(Time.Tics(x.Key), Notification.CreateOnNext(new MotionEnvelope(x.Value)))));
            
            return this;
        }

        public LightAutomationEnviromentBuilder WithRepeatedMotions(string roomUid, int numberOfMotions, TimeSpan waitTime)
        {
            long ticks = 0;

            for (int i = 0; i < numberOfMotions; i++)
            {
                ticks += Time.Tics((int)waitTime.TotalMilliseconds);

                _motionEvents.Add(new Recorded<Notification<MotionEnvelope>>(ticks, Notification.CreateOnNext(new MotionEnvelope(roomUid))));
            }

            return this;
        }

        public LightAutomationEnviromentBuilder WithRepeatedMotions(string roomUid, TimeSpan motionTime, TimeSpan waitTime)
        {
            int num = (int) (motionTime.TotalMilliseconds / waitTime.TotalMilliseconds);


            WithRepeatedMotions(roomUid, num, waitTime);

            return this;
        }

        public LightAutomationEnviromentBuilder WithLampEvents(params Recorded<Notification<PowerStateChangeEvent>>[] messages)
        {
            _lampEvents.AddRange(messages);
            return this;
        }

        public LightAutomationEnviromentBuilder WithPeriodicCheckTime(TimeSpan periodicCheckTimw)
        {
            _periodicCheckTime = periodicCheckTimw;
            return this;
        }

        public LightAutomationEnviromentBuilder WithTimeDuration(int timeDuration)
        {
            _timeDuration = timeDuration;
            return this;
        }

        public LightAutomationEnviromentBuilder WithServiceConfig(ServiceDTO service)
        {
            _serviceConfig = service;
            return this;
        }

        public IMapper Bootstrap(IMessageBroker messageBroker)
        {
            MapperConfiguration config = ConfigureMapper();

            var concurrencyProvider = new TestConcurrencyProvider(_scheduler);
            _container.RegisterInstance<IConcurrencyProvider>(concurrencyProvider);
            _container.RegisterInstance<ILogger<LightAutomationServiceProxy>>(new FakeLogger<LightAutomationServiceProxy>(_scheduler));
            _container.RegisterInstance(messageBroker);

            return config.CreateMapper();
        }

        private MapperConfiguration ConfigureMapper()
        {
            return new MapperConfiguration(p =>
            {
                p.CreateMap(typeof(ServiceDTO), typeof(LightAutomationServiceProxy)).ConstructUsingServiceLocator();
                p.CreateMap<AttachedPropertyDTO, AttachedProperty>();

                p.ShouldMapProperty = propInfo => (propInfo.CanWrite && propInfo.GetGetMethod(true).IsPublic) || propInfo.IsDefined(typeof(MapAttribute), false);
                p.ConstructServicesUsing(_container.GetInstance);
            });
        }

        public void Start()
        {
            var lampDictionary = CreateFakeLamps();

            var motionEvents = _scheduler.CreateColdObservable(_motionEvents.ToArray());
            var messageBroker = new FakeMessageBroker(motionEvents, lampDictionary);

            var checkTime = _periodicCheckTime ?? TimeSpan.FromMilliseconds(1000);
            var timeDuration = _timeDuration ?? 20;

            var mapper = Bootstrap(messageBroker);
            var actor = mapper.Map<ServiceDTO, LightAutomationServiceProxy>(_serviceConfig);

            _actorContext.Lamps = lampDictionary;
            _actorContext.Scheduler = _scheduler;
            _actorContext.MotionEvents = motionEvents;

            StartAndWait(actor);
        }

        private void StartAndWait(IActor actor)
        {
            _actorContext.PID = _actorContext.Context.SpawnNamed(Props.FromProducer(() => actor), "motionService");
            _actorContext.IsAlive();
        }

        private Dictionary<string, FakeMotionLamp> CreateFakeLamps()
        {
            var lampDictionary = new Dictionary<string, FakeMotionLamp>();

            foreach (var detector in _serviceConfig.ComponentsAttachedProperties)
            {
                var detectorName = detector.Properties[MotionProperties.Lamp];

                lampDictionary.Add(detectorName, new FakeMotionLamp(detectorName));
            }

            return lampDictionary;
        }


    }
}