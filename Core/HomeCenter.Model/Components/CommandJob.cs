﻿using HomeCenter.Model.Core;
using System;
using System.Threading.Tasks;

namespace HomeCenter.Model.Adapters
{
    public class CommandJob<T>
    {
        private TaskCompletionSource<T> _result { get; } = new TaskCompletionSource<T>();
        public ActorMessage Message { get; }
        public Task<T> Result => _result.Task;

        public CommandJob(ActorMessage message) => Message = message;

        public void SetResult(T result) => _result.SetResult(result);

        public void SetException(Exception error) => _result.SetException(error);
    }
}