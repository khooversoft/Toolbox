// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    /// <summary>
    /// Actor proxy built by RealProxy class in .NET.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerStepThrough]
    internal class ActorProxy<T> : RealProxy where T : IActor
    {
        private static readonly CorrelationVector _cv = new CorrelationVector(new Guid("{553F5F55-DFFA-48A3-A658-4C776C2789A1}"));
        private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);
        private readonly IActorBase _instance;
        private readonly IActorManager _manager;
        private readonly IWorkContext _workContext;
        private static readonly Tag _tag = new Tag(nameof(ActorProxy<T>));

        private ActorProxy(IActorBase instance, IActorManager manager)
                : base(typeof(T))
        {
            Verify.IsNotNull(nameof(instance), instance);
            Verify.IsNotNull(nameof(manager), manager);

            _instance = instance;
            _manager = manager;

            _workContext = new WorkContextBuilder()
                .Set(_tag)
                .Set(_cv)
                .Build();
        }

        /// <summary>
        /// Create transparent proxy for instance of actor class
        /// </summary>
        /// <param name="instance">instance of actor class</param>
        /// <param name="manager">actor manager</param>
        /// <returns>proxy</returns>
        public static T Create(IActorBase instance, IActorManager manager)
        {
            return (T)new ActorProxy<T>(instance, manager).GetTransparentProxy();
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="msg">message</param>
        /// <returns>message</returns>
        public override IMessage Invoke(IMessage msg)
        {
            Verify.IsNotNull(nameof(msg), msg);

            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                using (var scope = LockActor())
                {
                    var result = method.Invoke(_instance, methodCall.InArgs);
                    return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                }
            }
            catch (Exception ex)
            {
                ActorEventSource.Log.Error(_workContext, $"{ex.Message}", ex);

                if (ex is TargetInvocationException && ex.InnerException != null)
                {
                    return new ReturnMessage(ex.InnerException, msg as IMethodCallMessage);
                }

                return new ReturnMessage(ex, msg as IMethodCallMessage);
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
