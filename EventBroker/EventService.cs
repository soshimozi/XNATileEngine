using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBroker
{
    public class EventService
    {
        public static event EventHandler SubscriptionAdded;
        public static event EventHandler SubscriptionRemoved;

        private static volatile EventService instance;
        private static object syncRoot = new Object();
        private static Dictionary<string, List<Delegate>> subscriptions;

        private EventService()
        {
        }

        public static EventService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new EventService();
                            subscriptions = new Dictionary<string, List<Delegate>>();
                        }
                    }
                }

                return instance;
            }
        }

        private static Dictionary<string, List<Delegate>> Subscriptions
        {
            get { return EventService.subscriptions; }
            set
            {
                lock (syncRoot)
                {
                    EventService.subscriptions = value;
                }
            }
        }

        private static void OnSubscriptionAdded(EventArgs e)
        {
            if (SubscriptionAdded != null)
                SubscriptionAdded(Instance, e);
        }

        private static void OnSubscriptionRemoved(EventArgs e)
        {
            if (SubscriptionRemoved != null)
                SubscriptionRemoved(Instance, e);
        }

        public static void Subscribe(string eventName, Delegate method)
        {
            // get list of existing events
            List<Delegate> delegates = null;

            if (Subscriptions == null)
                Subscriptions = new Dictionary<string, List<Delegate>>();

            if (Subscriptions.ContainsKey(eventName))
            {
                delegates = subscriptions[eventName];
            }
            else
            {
                delegates = new List<Delegate>();
                Subscriptions.Add(eventName, delegates);
            }

            delegates.Add(method);
            OnSubscriptionAdded(EventArgs.Empty);
        }

        public static void Unsubscribe(string eventName, Delegate method)
        {
            if (Subscriptions.ContainsKey(eventName))
            {
                if (Subscriptions[eventName].Contains(method))
                {
                    Subscriptions[eventName].Remove(method);
                    OnSubscriptionRemoved(EventArgs.Empty);
                }

                if (Subscriptions[eventName].Count == 0)
                    Subscriptions.Remove(eventName);
            }
        }

        public static void FireEvent(string eventName, object sender, EventArgs e)
        {
            if (Subscriptions.ContainsKey(eventName))
            {
                for (int i = 0; i < Subscriptions[eventName].Count; i++)
                {
                    Delegate dg = Subscriptions[eventName][i];
                    DynamicInvoke(eventName, dg, sender, e);

                    if (!Subscriptions.ContainsKey(eventName))
                        break;
                }
            }
        }

        private static void DynamicInvoke(string eventName, Delegate dg, object sender, EventArgs e)
        {
            if (dg.Method != null)
            {
                if (dg.Target == null)
                {
                    Unsubscribe(eventName, dg);
                    return;
                }

                dg.DynamicInvoke(sender, e);
            }
        }
    }
}
