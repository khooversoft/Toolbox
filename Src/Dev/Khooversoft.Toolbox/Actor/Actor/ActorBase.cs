// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Base class for actors
    /// </summary>
    public abstract class ActorBase : IActorBase
    {
        private int _lockValue;
        private int _timerLockValue;
        private Timer _timer;
        private readonly Tag _tag = new Tag(nameof(ActorBase));
        private readonly IWorkContext _workContext = WorkContext.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actorKey">actor key</param>
        /// <param name="actorManager">actor manager</param>
        public ActorBase(ActorKey actorKey, IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorManager), actorManager);

            ActorKey = actorKey;
            ActorManager = actorManager;

            ActorEventSource.Log.ActorActivate(_workContext.WithTag(_tag), ActorKey, this.GetType());
        }

        /// <summary>
        /// Get actor key
        /// </summary>
        public ActorKey ActorKey { get; }

        /// <summary>
        /// Get actor manager
        /// </summary>
        public IActorManager ActorManager { get; }

        /// <summary>
        /// Activate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
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

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
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

        /// <summary>
        /// Dispose, virtual
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Event for on activate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        protected virtual Task OnActivate(IWorkContext context)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Event for on deactivate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        protected virtual Task OnDeactivate(IWorkContext context)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Time event
        /// </summary>
        /// <returns>task</returns>
        protected virtual Task OnTimer()
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Set timer notification of actor
        /// </summary>
        /// <param name="dueTime">due time, first event</param>
        /// <param name="period">every period</param>
        public void SetTimer(TimeSpan dueTime, TimeSpan period)
        {
            Verify.Assert(_timer == null, "Timer already running");

            _timer = new Timer(TimerCallback, null, dueTime, period);
            ActorEventSource.Log.ActorStartTimer(_workContext.WithMethodName(), ActorKey);
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public void StopTimer()
        {
            Timer t = Interlocked.Exchange(ref _timer, null);
            if (t != null)
            {
                t.Dispose();
                ActorEventSource.Log.ActorStopTimer(_workContext.WithMethodName(), ActorKey);
            }
        }

        /// <summary>
        /// Timer all back, through task
        /// </summary>
        /// <param name="obj">obj, not used</param>
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
