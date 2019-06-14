﻿using System.Threading.Tasks;
using ComposableAsync.Concurrent;

namespace ComposableAsync.Actors.DispatcherManagers
{
    internal sealed class TheadPoolFiberManager : IDispatcherManager
    {
        public bool DisposeDispatcher => true;

        public IDispatcher GetDispatcher() => Fiber.GetThreadPoolFiber();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
