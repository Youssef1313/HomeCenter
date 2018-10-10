﻿using System;
using System.Threading.Tasks;

namespace HomeCenter.TestRunner
{
    public abstract class Runner
    {
        private readonly string[] _tasks;

        internal Runner(string[] tasks)
        {
            _tasks = tasks;
        }

        public abstract Task RunTask(int taskId);

        public async Task Run()
        {
            Console.Clear();

            while (true)
            {
                var taskId = Menu();
                if (taskId == -1) continue;
                if (taskId == -2) break;
                try
                {
                    await RunTask(taskId).ConfigureAwait(false);
                }
                catch (Exception ee)
                {
                    ConsoleEx.WriteErrorLine(ee.ToString(), true);
                }
            }
        }

        public int Menu()
        {
            ConsoleEx.WriteTitleLine($"{GetType().Name} runner:");

            for (int i = 0; i < _tasks.Length; i++)
            {
                ConsoleEx.WriteMenuLine($"[{i}] {_tasks[i]}");
            }

            var runnerId = ConsoleEx.ReadNumber();
            if (runnerId == -2) return -2;

            if (runnerId >= _tasks.Length)
            {
                Console.WriteLine($"{runnerId} is outside of the scope");
                return -1;
            }

            return runnerId;
        }
    }
}