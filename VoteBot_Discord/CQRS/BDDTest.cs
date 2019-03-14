using System;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Aggregates;
using NUnit.Framework;
using System.Linq;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace VoteBot_Discord.CQRS
{
    public class BDDTest<TAggregate>
        where TAggregate : Aggregate, new()
    {
        private TAggregate agg;

        [SetUp]
        public void BDDTestSetup()
        {
            agg = new TAggregate();
        }

        protected void Test(IEnumerable given, Func<TAggregate, object> when, Action<object> then)
        {
            then(when(ApplyEvents(agg, given)));
        }

        protected IEnumerable Given(params object[] events)
        {
            return events;
        }

        protected Func<TAggregate, object> When<TCommand>(TCommand command)
        {
            return agg =>
            {
                try
                {
                    return DispatchCommand(command).Cast<object>().ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            };
        }

        protected Action<object> Then(params object[] expectedEvents)
        {
            return got =>
            {
                var gotEvents = got as object[];
                if (gotEvents != null)
                {
                    if (gotEvents.Length == expectedEvents.Length)
                    {
                        for (var i = 0; i < gotEvents.Length; i++)
                        {
                            if (gotEvents[i].GetType() == expectedEvents[i].GetType())
                            {
                                Assert.AreEqual(Serialize(expectedEvents[i]), Serialize(gotEvents[i]));
                            }
                            else
                            {
                                Assert.Fail($"Incorrect event in results: " +
                                    $"expected {expectedEvents[i].GetType().Name} " +
                                    $"but got a {gotEvents[i].GetType().Name}");
                            }
                        }
                    }
                    else if (gotEvents.Length < expectedEvents.Length)
                    {
                        Assert.Fail($"Expected event(s) missing: {string.Join(", ", EventDiff(expectedEvents, gotEvents))}");
                    }
                    else
                    {
                        Assert.Fail($"Unexpected event(s) emitted: {string.Join(", ", EventDiff(gotEvents, expectedEvents))}");
                    }
                }
                else if (got is CommandHandlerNotDefiendException)
                {
                    Assert.Fail((got as Exception).Message);
                }
                else
                {
                    Assert.Fail($"Expected events, but got exception {got.GetType().Name}");
                }
            };
        }

        private string[] EventDiff(object[] a, object[] b)
        {
            var diff = a.Select(evt => evt.GetType().Name).ToList();
            foreach(var remove in b.Select(evt => evt.GetType().Name))
            {
                diff.Remove(remove);
            }
            return diff.ToArray();
        }

        protected Action<object> ThenFailWith<TException>()
        {
            return got =>
            {
                if (got is TException)
                {
                    Assert.Pass("Got correct exception type");
                }
                else if (got is CommandHandlerNotDefiendException)
                {
                    Assert.Fail((got as Exception).Message);
                }
                else if (got is Exception)
                {
                    Assert.Fail($"Expected exception {typeof(TException).Name}, but got exception {got.GetType().Name}");
                }
                else
                {
                    Assert.Fail($"Expected exception {typeof(TException).Name}, but got event result");
                }
            };
        }

        private IEnumerable DispatchCommand<TCommand>(TCommand c)
        {
            var handler = agg as IHandleCommand<TCommand>; 
            if(handler == null)
            {
                throw new CommandHandlerNotDefiendException($"Aggregate {agg.GetType().Name} does not yet handle command {c.GetType().Name}");
            }
            return handler.Handle(c);
        }

        private TAggregate ApplyEvents(TAggregate aggregate, IEnumerable events)
        {
            aggregate.ApplyEvents(events);
            return aggregate;
        }

        private string Serialize(object obj)
        {
            var ser = new XmlSerializer(obj.GetType());
            var ms = new MemoryStream();
            ser.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return new StreamReader(ms).ReadToEnd();
        }

        private class CommandHandlerNotDefiendException : Exception
        {
            public CommandHandlerNotDefiendException(string msg) : base(msg) { }
        }
    }
}
