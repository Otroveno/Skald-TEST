using System;
using System.Collections.Generic;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.UI
{
    /// <summary>
    /// Service de notifications minimal.
    /// Affiche des messages à l'utilisateur (success, error, warning, info).
    /// Queue de notifications avec TTL.
    /// </summary>
    public class NotificationService
    {
        private readonly List<Notification> _notificationList = new List<Notification>();
        private readonly object _lock = new object();
        private const int MAX_NOTIFICATIONS = 5;
        private const float DEFAULT_TTL = 3.0f; // 3 secondes

        public event Action<Notification>? OnNotificationAdded;
        public event Action<Notification>? OnNotificationRemoved;

        public NotificationService()
        {
            Logger.Info("NotificationService", "NotificationService initialized");
        }

        /// <summary>
        /// Affiche une notification.
        /// </summary>
        public void Show(string message, ResultType type = ResultType.Info, float ttl = DEFAULT_TTL)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                var notification = new Notification
                {
                    Message = message,
                    Type = type,
                    Timestamp = GetCurrentTime(),
                    TTL = ttl
                };

                lock (_lock)
                {
                    // Limiter le nombre de notifications
                    while (_notificationList.Count >= MAX_NOTIFICATIONS)
                    {
                        var removed = _notificationList[0];
                        _notificationList.RemoveAt(0);
                        OnNotificationRemoved?.Invoke(removed);
                    }

                    _notificationList.Add(notification);
                    Logger.Debug("NotificationService", $"Notification added: [{type}] {message}");
                }

                OnNotificationAdded?.Invoke(notification);
            }
            catch (Exception ex)
            {
                Logger.Error("NotificationService", "Error showing notification", ex);
            }
        }

        /// <summary>
        /// Affiche une notification de succès.
        /// </summary>
        public void ShowSuccess(string message, float ttl = DEFAULT_TTL)
        {
            Show(message, ResultType.Success, ttl);
        }

        /// <summary>
        /// Affiche une notification d'erreur.
        /// </summary>
        public void ShowError(string message, float ttl = DEFAULT_TTL)
        {
            Show(message, ResultType.Error, ttl);
        }

        /// <summary>
        /// Affiche une notification d'avertissement.
        /// </summary>
        public void ShowWarning(string message, float ttl = DEFAULT_TTL)
        {
            Show(message, ResultType.Warning, ttl);
        }

        /// <summary>
        /// Affiche une notification d'info.
        /// </summary>
        public void ShowInfo(string message, float ttl = DEFAULT_TTL)
        {
            Show(message, ResultType.Info, ttl);
        }

        /// <summary>
        /// Update tick: retire les notifications expirées.
        /// </summary>
        public void OnTick(float deltaTime)
        {
            lock (_lock)
            {
                var currentTime = GetCurrentTime();
                var toRemove = new List<Notification>();

                foreach (var notification in _notificationList)
                {
                    if (currentTime - notification.Timestamp >= notification.TTL)
                    {
                        toRemove.Add(notification);
                    }
                }

                foreach (var notification in toRemove)
                {
                    _notificationList.Remove(notification);
                    OnNotificationRemoved?.Invoke(notification);
                    Logger.Debug("NotificationService", $"Notification expired: {notification.Message}");
                }
            }
        }

        /// <summary>
        /// Clear toutes les notifications.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                foreach (var notification in new List<Notification>(_notificationList))
                {
                    OnNotificationRemoved?.Invoke(notification);
                }
                _notificationList.Clear();
                Logger.Debug("NotificationService", "All notifications cleared");
            }
        }

        /// <summary>
        /// Récupère toutes les notifications actives (copie).
        /// </summary>
        public List<Notification> GetActiveNotifications()
        {
            lock (_lock)
            {
                return new List<Notification>(_notificationList);
            }
        }

        private float GetCurrentTime()
        {
            return (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        }
    }

    /// <summary>
    /// Modèle de notification.
    /// </summary>
    public class Notification
    {
        public string Message { get; set; } = "";
        public ResultType Type { get; set; } = ResultType.Info;
        public float Timestamp { get; set; }
        public float TTL { get; set; }
    }
}
