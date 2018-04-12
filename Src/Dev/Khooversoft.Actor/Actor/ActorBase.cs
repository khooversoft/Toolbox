// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    public abstract class ActorBase : IActorBase
    {
        private int _lockValue;
        private int _timerLockValue;
        private Timer _timer;
        private readonly Tag _tag = new Tag(nameof(ActorBase));
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public ActorBase(ActorKey actorKey, IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorManager), actorManager);

            ActorKey = actorKey;
            ActorManager = actorManager;

            ActorEventSource.Log.ActorActivate(_workContext.WithTag(_tag), ActorKey, this.GetType());
        }

        public ActorKey ActorKey { get; }

        public IActorManager ActorManager { get; }

        public async Task ActivateAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            int currentValue = Interlocked.CompareExchange(ref _lockValue, 1, 0);
            if (currentValue != 0)
            {
                return;
            }

            ActorEventSource.Log.ActorActivate(context.WithTag(_tag), ActorKey, this.GetType());
            await OnActivate(context);
        }

        public async Task DeactivateAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            int currentValue = Interlocked.CompareExchange(ref _lockValue, 0, 1);
            if (currentValue != 1)
            {
                return;
            }

            ActorEventSource.Log.ActorDeactivate(context.WithTag(_tag), ActorKey, this.GetType());
            StopTimer();
            await OnDeactivate(context);
        }

        public virtual void Dispose()
        {
        }

        protected virtual Task OnActivate(IWorkContext context)
        {
            return Task.FromResult(0);
        }

        protected virtual Task OnDeactivate(IWorkContext context)
        {
            return Task.FromResult(0);
        }

        protected virtual Task OnTimer()
        {
            return Task.FromResult(0);
        }

        public void SetTimer(TimeSpan dueTime, TimeSpan period)
        {
            Verify.Assert(_timer == null, "Timer already running");

            _timer = new Timer(TimerCallback, null, dueTime, period);
            ActorEventSource.Log.ActorStartTimer(_workContext.WithMethodName(), ActorKey);
        }

        public void StopTimer()
        {
            Timer t = Interlocked.Exchange(ref _timer, null);
            if (t != null)
            {
                t.Dispose();
                ActorEventSource.Log.ActorStopTimer(_workContext.WithMethodName(), ActorKey);
            }
        }

        private void TimerCallback(object obj)
        {
            int currentValue = Interlocked.CompareExchange(ref _timerLockValue, 1, 0);
            if (currentValue != 0)
            {
                return;
            }

            try
            {
                ActorEventSource.Log.ActorStartTimer(_workContext.WithMethodName(), ActorKey);
                OnTimer().GetAwaiter().GetResult();
            }
            finally
            {
                Interlocked.Exchange(ref _timerLockValue, 0);
            }
        }
    }
}
