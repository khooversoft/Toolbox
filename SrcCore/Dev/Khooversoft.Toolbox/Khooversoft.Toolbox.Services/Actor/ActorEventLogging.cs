using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    public static class ActorEventLogging
    {
        public static void ActorCreated(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(actorType), actorType)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorCreated), dimensions);
            configuration.WorkContext.EventLog.TrackMetric(context, nameof(ActorCreated), dimensions);
        }

        public static void ActorActivate(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(actorType), actorType)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorActivate), dimensions);
            configuration.WorkContext.EventLog.TrackMetric(context, nameof(ActorActivate), dimensions);
        }

        public static void ActorDeactivate(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(actorType), actorType)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorDeactivate), dimensions);
        }

        public static void ActorCalling(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string methodName, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(actorType), actorType)
                .Set(nameof(methodName), methodName)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorCalling), dimensions);
        }

        public static void ActorCalled(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type interfaceType, string methodName, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(interfaceType), interfaceType);
            Verify.IsNotEmpty(nameof(methodName), methodName);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(interfaceType), interfaceType)
                .Set(nameof(methodName), methodName)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorCalled), dimensions);
        }

        public static void ActorRegistered(this IActorConfiguration configuration, IWorkContext context, Type interfaceType, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(interfaceType), interfaceType);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(interfaceType), interfaceType)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorRegistered), dimensions);
        }

        public static void ActorStartTimer(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorStartTimer), dimensions);
        }

        public static void ActorStopTimer(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorStopTimer), dimensions);
        }

        public static void ActorTimerCallback(this IActorConfiguration configuration, IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Set(nameof(actorKey), actorKey)
                .Set(nameof(message), message)
                .Build();

            configuration.WorkContext.EventLog.Verbose(context, nameof(ActorTimerCallback), dimensions);
        }
    }
}
