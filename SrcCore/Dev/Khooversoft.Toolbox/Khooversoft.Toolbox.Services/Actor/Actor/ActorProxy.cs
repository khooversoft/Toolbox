// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor proxy built by RealProxy class in .NET.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[DebuggerStepThrough]
    public class ActorProxy<T> : DispatchProxy where T : IActor
    {
        private static readonly CorrelationVector _cv = new CorrelationVector("Toolbox-ActorProxy");
        private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);
        private IActorBase _instance;
        private IActorManager _manager;
        private IWorkContext _workContext;
        private static readonly Tag _tag = new Tag(nameof(ActorProxy<T>));

        public ActorProxy()
        {
        }

        /// <summary>
        /// Create transparent proxy for instance of actor class
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="instance">instance of actor class</param>
        /// <param name="manager">actor manager</param>
        /// <returns>proxy</returns>
        public static T Create(IWorkContext context, IActorBase instance, IActorManager manager)
        {
            Debugger.Break();

            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(instance), instance);
            Verify.IsNotNull(nameof(manager), manager);

            object proxyObject = Create<T, ActorProxy<T>>();

            ActorProxy<T> proxy = (ActorProxy<T>)proxyObject;
            proxy._instance = instance;
            proxy._manager = manager;

            proxy._workContext = new WorkContextBuilder(context)
                .Set(_tag)
                .Set(_cv)
                .Build();

            return (T)proxyObject;
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                using (var scope = LockActor())
                {
                    return targetMethod.Invoke(_instance, args);
               }
            }
            catch (Exception ex)
            {
                _workContext.EventLog.Error(_workContext, ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Create actor lock scope
        /// </summary>
        /// <returns>disposable scope for semaphore</returns>
        private IDisposable LockActor()
        {
            _lockSemaphore.Wait(_manager.Configuration.ActorCallTimeout);
            return new Scope<ActorProxy<T>>(this, x => x._lockSemaphore.Release());
        }
    }
}
