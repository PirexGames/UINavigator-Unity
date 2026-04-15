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
        private List<UniTask> _animationTasks = new(4);
        private bool _isProcessingMono = false;

        protected override string RegistryKey => base.RegistryKey + "_" + mode.ToString();

        public async UniTask<T> Show<T>(string key, string content, float duration = 2f) where T : ToastView
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
                    
                    _animationTasks.Clear();
                    _animationTasks.Add(oldToast.OnHide());
                    await AwaitTasks(_animationTasks, true);
                    
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
            await toast.Init(content);
            
            _isProcessingMono = false;
            HandleAutoClose(toast, duration).Forget();
            
            return toast;
        }

        private async UniTaskVoid HandleAutoClose(ToastView toast, float duration)
        {
            await UniTask.Delay((int)(duration * 1000), cancellationToken: toast.SessionToken);
            
            if (!toast) return;

            // Manual index check instead of Contains/Remove for better performance
            int index = -1;
            for (int i = 0; i < _activeToasts.Count; i++)
            {
                if (_activeToasts[i] == toast) { index = i; break; }
            }
            if (index == -1) return;

            _activeToasts.RemoveAt(index);
            
            _animationTasks.Clear();
            _animationTasks.Add(toast.OnHide());
            await AwaitTasks(_animationTasks, true);
            
            ReleaseToast(toast);
        }

        protected async UniTask AwaitTasks(List<UniTask> tasks, bool awaitAnimation)
        {
            if (tasks.Count == 0) return;
            if (awaitAnimation)
            {
                if (tasks.Count == 1) await tasks[0];
                else await UniTask.WhenAll(tasks);
            }
            else
            {
                if (tasks.Count == 1) tasks[0].Forget();
                else UniTask.WhenAll(tasks.ToArray()).Forget();
            }
            tasks.Clear();
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
    }
}
