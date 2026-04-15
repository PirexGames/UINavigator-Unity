using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class AlertContainer : BackdropStackContainer<AlertView>
    {
        /// <summary>
        /// Instance method to show an alert, synchronized with Toasts.Show style.
        /// Call via UINavigator.Alerts.Show(...)
        /// </summary>
        public virtual async UniTask<AlertView> Show(string key, string title, string content, Action<CallbackStatus> callback = null, 
            string buttonPositive = "", string buttonNegative = "", string buttonNeutral = "", bool closeOnClickBackdrop = false)
        {
            return await Push<AlertView>(
                key, 
                ViewPushMode.Standard, 
                true, 
                (alert) => {
                    alert.CloseOnClickBackdrop = closeOnClickBackdrop;
                    alert.Init(title, content, callback, buttonPositive, buttonNegative, buttonNeutral);
                }
            );
        }
    }
}
