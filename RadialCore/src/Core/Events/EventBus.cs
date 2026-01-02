using System;
using System.Collections.Generic;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.Events
{
    /// <summary>
    /// Event Bus centralisé pour communication entre plugins et Core.
    /// Pattern: Publish/Subscribe avec typed events.
    /// Thread-safe, fail-safe (subscribers ne cassent pas le bus).
    /// </summary>
    public class EventBus
    {
        private static EventBus? _instance;
        private static readonly object _instanceLock = new object();

        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventBus();
                        }
                    }
                }
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        private readonly object _subscribersLock = new object();

        private EventBus()
        {
            Logger.Info("EventBus", "EventBus initialized");
        }

        /// <summary>
        /// Subscribe à un type d'event.
        /// </summary>
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_subscribersLock)
            {
                var eventType = typeof(TEvent);
                
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType] = new List<Delegate>();
                }

                _subscribers[eventType].Add(handler);
                Logger.Debug("EventBus", $"Subscribed to event: {eventType.Name}");
            }
        }

        /// <summary>
        /// Unsubscribe d'un type d'event.
        /// </summary>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_subscribersLock)
            {
                var eventType = typeof(TEvent);
                
                if (_subscribers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);
                    Logger.Debug("EventBus", $"Unsubscribed from event: {eventType.Name}");
                }
            }
        }

        /// <summary>
        /// Publish un event à tous les subscribers.
        /// Fail-safe: si un subscriber throw, les autres sont quand même notifiés.
        /// </summary>
        public void Publish<TEvent>(TEvent eventData) where TEvent : class
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            List<Delegate>? handlersCopy;
            
            lock (_subscribersLock)
            {
                var eventType = typeof(TEvent);
                
                if (!_subscribers.TryGetValue(eventType, out var handlers) || handlers.Count == 0)
                {
                    Logger.Debug("EventBus", $"No subscribers for event: {eventType.Name}");
                    return;
                }

                // Copie pour éviter lock pendant execution
                handlersCopy = new List<Delegate>(handlers);
            }

            Logger.Debug("EventBus", $"Publishing event: {typeof(TEvent).Name} to {handlersCopy.Count} subscribers");

            foreach (var handler in handlersCopy)
            {
                try
                {
                    ((Action<TEvent>)handler)(eventData);
                }
                catch (Exception ex)
                {
                    Logger.Error("EventBus", 
                        $"Exception in event subscriber for {typeof(TEvent).Name}", 
                        ex);
                }
            }
        }

        /// <summary>
        /// Clear tous les subscribers (shutdown).
        /// </summary>
        public void Clear()
        {
            lock (_subscribersLock)
            {
                Logger.Info("EventBus", $"Clearing {_subscribers.Count} event types");
                _subscribers.Clear();
            }
        }

        /// <summary>
        /// Retourne le nombre de subscribers pour un type d'event.
        /// </summary>
        public int GetSubscriberCount<TEvent>() where TEvent : class
        {
            lock (_subscribersLock)
            {
                var eventType = typeof(TEvent);
                return _subscribers.TryGetValue(eventType, out var handlers) ? handlers.Count : 0;
            }
        }
    }

    // ===== Events Prédéfinis =====

    /// <summary>
    /// Event: Menu radial ouvert.
    /// </summary>
    public class RadialMenuOpenedEvent
    {
        public float Timestamp { get; set; }
        public Contracts.Models.MenuContext Context { get; set; } = null!;
    }

    /// <summary>
    /// Event: Menu radial fermé.
    /// </summary>
    public class RadialMenuClosedEvent
    {
        public float Timestamp { get; set; }
        public bool WasActionExecuted { get; set; }
    }

    /// <summary>
    /// Event: Action exécutée.
    /// </summary>
    public class ActionExecutedEvent
    {
        public string ActionId { get; set; } = "";
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public float Timestamp { get; set; }
    }

    /// <summary>
    /// Event: Context refreshé.
    /// </summary>
    public class ContextRefreshedEvent
    {
        public Contracts.Models.MenuContext Context { get; set; } = null!;
        public float Timestamp { get; set; }
    }

    /// <summary>
    /// Event: Plugin chargé.
    /// </summary>
    public class PluginLoadedEvent
    {
        public string PluginId { get; set; } = "";
        public string PluginName { get; set; } = "";
        public string Version { get; set; } = "";
    }

    /// <summary>
    /// Event: Plugin désactivé (circuit breaker).
    /// </summary>
    public class PluginDisabledEvent
    {
        public string PluginId { get; set; } = "";
        public string Reason { get; set; } = "";
        public int FailureCount { get; set; }
    }
}
