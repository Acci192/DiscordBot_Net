using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Aggregates;
using System.Reflection;
using System.Linq;

namespace VoteBot_Discord.CQRS
{
    public class MessageDispatcher
    {
        private Dictionary<Type, Action<object>> CommandHandlers =
            new Dictionary<Type, Action<object>>();
        private Dictionary<Type, List<Action<object>>> EventSubscribers =
            new Dictionary<Type, List<Action<object>>>();
        private IEventStore EventStore;

        public MessageDispatcher(IEventStore es)
        {
            EventStore = es;
        }

        public void SendCommand<TCommand>(TCommand c)
        {
            if (CommandHandlers.ContainsKey(typeof(TCommand)))
                CommandHandlers[typeof(TCommand)](c);
            else
                throw new Exception($"No command handler registred for {typeof(TCommand).Name}");
        }

        internal void SendCommand(object createPoll, object cmd)
        {
            throw new NotImplementedException();
        }

        private void PublishEvent(object e)
        {
            var eventType = e.GetType();
            if (EventSubscribers.ContainsKey(eventType))
            {
                foreach(var sub in EventSubscribers[eventType])
                {
                    sub(e);
                }
            }
        }

        public void AddHandlerFor<TCommand, TAggregate>()
            where TAggregate : Aggregate, new()
        {
            if (CommandHandlers.ContainsKey(typeof(TCommand)))
                throw new Exception($"Command handler already registered for {typeof(TCommand).Name}");

            CommandHandlers.Add(typeof(TCommand), c =>
            {
                var agg = new TAggregate();

                agg.Id = ((dynamic)c).Id;
                agg.ApplyEvents(EventStore.LoadEventsFor<TAggregate>(agg.Id));

                var resultEvents = new ArrayList();
                foreach (var e in (agg as IHandleCommand<TCommand>).Handle((TCommand)c))
                {
                    resultEvents.Add(e);
                }

                if (resultEvents.Count > 0)
                {
                    EventStore.SaveEventsFor<TAggregate>(agg.Id, agg.EventsLoaded, resultEvents);
                }

                foreach (var e in resultEvents)
                {
                    PublishEvent(e);
                }
            });
        }

        public void AddSubscriberFor<TEvent>(ISubscribeTo<TEvent> subscriber)
        {
            if (!EventSubscribers.ContainsKey(typeof(TEvent)))
            {
                EventSubscribers.Add(typeof(TEvent), new List<Action<object>>());
            }
            EventSubscribers[typeof(TEvent)].Add(e => subscriber.Handle((TEvent)e));
        }

        public void ScanAssembly(Assembly ass)
        {
            var handlers =
                from t in ass.GetTypes()
                from i in t.GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(IHandleCommand<>)
                let args = i.GetGenericArguments()
                select new
                {
                    CommandType = args[0],
                    AggregateType = t
                };

            foreach(var h in handlers)
            {
                this.GetType().GetMethod("AddHandlerFor")
                    .MakeGenericMethod(h.CommandType, h.AggregateType)
                    .Invoke(this, new object[] { });
            }

            var subscriber =
                from t in ass.GetTypes()
                from i in t.GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(ISubscribeTo<>)
                select new
                {
                    Type = t,
                    EventType = i.GetGenericArguments()[0]
                };

            foreach(var s in subscriber)
            {
                this.GetType().GetMethod("AddSubcsriberFor")
                    .MakeGenericMethod(s.EventType)
                    .Invoke(this, new object[] { CreateInstanceOf(s.Type) });
            }
        }

        public void ScanInstance(object instance)
        {
            var handlers = 
                from i in instance.GetType().GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(IHandleCommand<>)
                let args = i.GetGenericArguments()
                select new
                {
                    CommandType = args[0],
                    AggregateType = instance.GetType()
                };

            foreach(var h in handlers)
            {
                this.GetType().GetMethod("AddHandlerFor")
                    .MakeGenericMethod(h.CommandType, h.AggregateType)
                    .Invoke(this, new object[] { });
            }

            var subscriber =
                from i in instance.GetType().GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(ISubscribeTo<>)
                select i.GetGenericArguments()[0];

            foreach(var s in subscriber)
            {
                this.GetType().GetMethod("AddSubscriberFor")
                    .MakeGenericMethod(s)
                    .Invoke(this, new object[] { instance });
            }
        }

        private object CreateInstanceOf(Type t)
        {
            return Activator.CreateInstance(t);
        }
    }
}
