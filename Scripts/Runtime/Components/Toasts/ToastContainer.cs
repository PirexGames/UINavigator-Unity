using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public enum ToastMode { Mono, Multiple }

    public class ToastContainer : UIContainer<ToastView>
    {
        [SerializeField] private ToastMode mode = ToastMode.Multiple;
        private List<ToastView> _activeToasts = new();
        private bool _isProcessingMono = false;

        protected override string RegistryKey => base.RegistryKey + "_" + mode.ToString();

        public async UniTask Show(string key, string content, float duration = 2f)
        {
            await Show<ToastView>(key, content, duration);
        }

        public async UniTask<T> Show<T>(string key, string content, float duration = 2f, Action<T> onInit = null) where T : ToastView
        {
            // Lock-like mechanism: Wait if already processing
            while (_isProcessingMono)
            {
                await UniTask.Yield();
            }

            if (mode == ToastMode.Mono)
            {
                _isProcessingMono = true;

                if (_activeToasts.Count > 0)
                {
                    var oldToast = _activeToasts[0];
                    _activeToasts.RemoveAt(0);
                    
                    var tasks = ListPool<UniTask>.Get();
                    tasks.Add(oldToast.OnHide());
                    await AwaitTasks(tasks, true);
                    
                    ReleaseToast(oldToast);
                }
            }

            var go = await SpawnView(key, TransformContainer);
            var toast = go.GetComponent<T>();
            if (toast == null)
            {
                _isProcessingMono = false;
                return null;
            }

            _activeToasts.Add(toast);
            toast.NameAddressable = key;
            toast.ApplyPivotCenter();
            await toast.DoInitialize();
            onInit?.Invoke(toast);
            await toast.Init(content);
            
            _isProcessingMono = false;
            HandleAutoClose(toast, duration).Forget();
            
            return toast;
        }

        private async UniTaskVoid HandleAutoClose(ToastView toast, float duration)
        {
            await UniTask.Delay((int)(duration * 1000), ignoreTimeScale: true, cancellationToken: toast.SessionToken);
            
            if (!toast) return;

            // Manual index check instead of Contains/Remove for better performance
            int index = -1;
            for (int i = 0; i < _activeToasts.Count; i++)
            {
                if (_activeToasts[i] == toast) { index = i; break; }
            }
            if (index == -1) return;

            _activeToasts.RemoveAt(index);
            
            var tasks = ListPool<UniTask>.Get();
            tasks.Add(toast.OnHide());
            await AwaitTasks(tasks, true);
            
            ReleaseToast(toast);
        }

        private void ReleaseToast(ToastView toast)
        {
            if (toast == null) return;
            var go = toast.gameObject;
            
            if (toast.IsPooling)
            {
                UIContainer.ReturnToPool(toast.NameAddressable, go);
            }
            else
            {
                // Manual destroy for non-pooled toasts
                if (go != null)
                {
                    Destroy(go);
                }
            }
        }

        public override async UniTask Close(UIView view, bool awaitAnimation = true)
        {
            if (view is ToastView toast && _activeToasts.Contains(toast))
            {
                _activeToasts.Remove(toast);
                
                var tasks = ListPool<UniTask>.Get();
                tasks.Add(toast.OnHide());
                await AwaitTasks(tasks, awaitAnimation);
                
                ReleaseToast(toast);
            }
        }
    }
}
