using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public abstract class UIStackContainer<TView> : UIContainer<TView> where TView : UIView
    {
        private readonly List<TView> _stack = new();
        public int Count => _stack.Count;

        public virtual async UniTask<T> Push<T>(string key, ViewPushMode mode = ViewPushMode.Standard, bool awaitAnimation = true, Action<T> onInit = null) where T : TView
        {
            TView instance = null;

            switch (mode)
            {
                case ViewPushMode.Standard:
                    instance = await InternalSpawn<T>(key);
                    break;

                case ViewPushMode.BringToFront:
                    for (int i = 0; i < _stack.Count; i++)
                    {
                        if (_stack[i].NameAddressable == key)
                        {
                            instance = _stack[i];
                            break;
                        }
                    }

                    if (instance != null)
                    {
                        _stack.Remove(instance);
                    }
                    else
                    {
                        instance = await InternalSpawn<T>(key);
                    }
                    break;

                case ViewPushMode.SetRoot:
                    await ClearStack();
                    instance = await InternalSpawn<T>(key);
                    break;
            }

            if (instance == null) return null;
            onInit?.Invoke(instance as T);

            var tasks = new List<UniTask>();

            // Handle current top (Parallel Hide)
            if (_stack.Count > 0)
            {
                var currentTop = _stack[^1];
                tasks.Add(OnBeforeHideTop(currentTop, instance));
            }

            _stack.Add(instance);
            instance.transform.SetAsLastSibling();

            // Handle new view (Parallel Show)
            var showTask = OnShowView(instance);
            tasks.Add(showTask);

            if (awaitAnimation)
            {
                await UniTask.WhenAll(tasks);
            }
            else
            {
                UniTask.WhenAll(tasks).Forget();
            }

            return instance as T;
        }

        public virtual async UniTask Pop(bool awaitAnimation = true)
        {
            if (_stack.Count == 0) return;

            var top = _stack[^1];
            _stack.RemoveAt(_stack.Count - 1);

            var tasks = new List<UniTask>();

            // Sequence: Animation -> Destroy/Release
            async UniTask HideAndCleanup(TView view)
            {
                await OnHideView(view);
                OnViewDestroyed(view);
            }

            tasks.Add(HideAndCleanup(top));

            // Handle the below view (Parallel PopEnter)
            if (_stack.Count > 0)
            {
                var newTop = _stack[^1];
                tasks.Add(OnBeforeShowNewTop(newTop, top));
            }

            if (awaitAnimation)
            {
                await UniTask.WhenAll(tasks);
            }
            else
            {
                UniTask.WhenAll(tasks).Forget();
            }
        }

        public virtual async UniTask Pop(TView view, bool awaitAnimation = true)
        {
            if (!view || !_stack.Contains(view)) return;

            // If it's the top one, use normal Pop
            if (_stack[^1] == view)
            {
                await Pop(awaitAnimation);
                return;
            }

            // A: Pop from middle but ensure cleanup
            _stack.Remove(view);
            
            async UniTask HideAndCleanup(TView v)
            {
                await OnHideView(v);
                OnViewDestroyed(v);
            }

            var finalizeTask = HideAndCleanup(view);
            if (awaitAnimation) await finalizeTask;
            else finalizeTask.Forget();
        }

        protected virtual async UniTask<T> InternalSpawn<T>(string key) where T : TView
        {
            var go = await SpawnView(key, TransformContainer);
            var view = go.GetComponent<T>();
            if (view) 
            {
                view.NameAddressable = key;
                // EXPLICIT INITIALIZATION: Always call DoInitialize regardless of pooling
                await view.DoInitialize();
            }
            return view;
        }

        protected virtual async UniTask ClearStack()
        {
            foreach (var view in _stack)
            {
                OnViewDestroyed(view);
            }
            _stack.Clear();
            await UniTask.CompletedTask;
        }

        protected virtual void OnViewDestroyed(TView view)
        {
            if (view == null) return;
            var go = view.gameObject;
            
            if (view.IsPooling)
            {
                UIContainer.ReturnToPool(view.NameAddressable, go);
            }
            else
            {
                // Normal destroy logic
                if (go)
                {
                    Destroy(go);
                }
            }
        }

        // Animation hooks to be overridden by Screen/Modal
        protected abstract UniTask OnShowView(TView view);
        protected abstract UniTask OnHideView(TView view);
        protected abstract UniTask OnBeforeHideTop(TView currentTop, TView nextTop);
        protected abstract UniTask OnBeforeShowNewTop(TView newTop, TView oldTop);
    }
}
