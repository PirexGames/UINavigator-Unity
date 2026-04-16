using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public enum ContainerNameMode { Default, Custom }

    public abstract class UIContainer : MonoBehaviour
    {
        [SerializeField] protected ContainerNameMode nameMode = ContainerNameMode.Default;
        [SerializeField] protected string containerName = "Default";

        public string ContainerName => containerName;
        public ContainerNameMode NameMode => nameMode;
        protected virtual string RegistryKey => containerName;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (nameMode == ContainerNameMode.Default)
            {
                containerName = "Default";
            }
        }
#endif

        private static Dictionary<System.Type, Dictionary<string, UIContainer>> _registry = new();
        private static Dictionary<string, Stack<GameObject>> _pool = new();
        private static Transform _poolRoot;
        private System.Threading.CancellationTokenSource _cts;

        private static void EnsurePoolRoot()
        {
            if (_poolRoot != null) return;
            var go = new GameObject("[UINavigator_Pool]");
            _poolRoot = go.transform;
            go.SetActive(false);
            DontDestroyOnLoad(go);
        }

        public static GameObject GetFromPool(string key)
        {
            if (_pool.TryGetValue(key, out var stack))
            {
                while (stack.Count > 0)
                {
                    var go = stack.Pop();
                    // Ensure we ONLY get an inactive object that still exists
                    if (go && !go.activeSelf)
                    {
                        return go;
                    }
                }
            }
            return null;
        }

        public static void ReturnToPool(string key, GameObject go)
        {
            if (go == null) return;
            
            EnsurePoolRoot();
            if (!_pool.ContainsKey(key)) _pool[key] = new Stack<GameObject>();
            
            // Limit pool size to 20
            if (_pool[key].Count >= 20)
            {
                Destroy(go);
                return;
            }

            go.SetActive(false); // Explicitly deactivate before pooling
            go.transform.SetParent(_poolRoot, false);
            go.transform.localScale = Vector3.one; // Reset scale to prevent cumulative scaling bugs
            _pool[key].Push(go);
        }

        /// <summary>
        /// Clears all pooled objects and destroys their GameObjects
        /// </summary>
        public static void ReleaseAllPools()
        {
            if (_pool == null) return;

            foreach (var stack in _pool.Values)
            {
                while (stack.Count > 0)
                {
                    var go = stack.Pop();
                    if (go != null) Destroy(go);
                }
            }
            _pool.Clear();

            // IMPORTANT: Also clear Addressable prefab caches to free RAM
            AddressableHelper<GameObject>.ClearCache();

            if (_poolRoot != null)
            {
                Destroy(_poolRoot.gameObject);
                _poolRoot = null;
            }
        }

        public System.Threading.CancellationToken DestroyToken 
        {
            get
            {
                if (_cts == null) _cts = new System.Threading.CancellationTokenSource();
                return _cts.Token;
            }
        }

        protected virtual void Awake()
        {
            if (_cts == null) _cts = new System.Threading.CancellationTokenSource();
            Register();
        }

        protected virtual void OnDestroy()
        {
            Unregister();
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        protected virtual void Register()
        {
            RegisterAs(GetType(), RegistryKey);
        }

        protected void RegisterAs(System.Type type, string name)
        {
            if (!_registry.ContainsKey(type))
                _registry[type] = new Dictionary<string, UIContainer>();

            _registry[type][name] = this;
        }

        protected virtual void Unregister()
        {
            UnregisterFrom(GetType(), RegistryKey);
        }

        protected void UnregisterFrom(System.Type type, string name)
        {
            if (_registry.ContainsKey(type))
            {
                if (_registry[type].ContainsKey(name) && _registry[type][name] == this)
                {
                    _registry[type].Remove(name);
                }
            }
        }

        public static T Get<T>(string name = "Default") where T : UIContainer
        {
            if (_registry.TryGetValue(typeof(T), out var dict))
            {
                if (dict.TryGetValue(name, out var container))
                    return (T)container;
            }
            return null;
        }

        public abstract RectTransform RectContainer { get; }
        public abstract Transform TransformContainer { get; }

        public static async UniTask<GameObject> SpawnView(string key, Transform parent)
        {
            // Check pool first
            var pooledView = GetFromPool(key);
            if (pooledView)
            {
                pooledView.transform.SetParent(parent, false);
                
                // Ensure alpha is 0 before activating to prevent flickering
                var cg = pooledView.GetComponent<CanvasGroup>();
                if (cg)
                {
                    cg.alpha = 0;
                    cg.blocksRaycasts = false;
                }
                
                pooledView.SetActive(true);
                return pooledView;
            }

            // Use project utility helper for pre-loading prefabs
            var prefab = await SpawnGOHelper.Spawn(key, parent);
            
            // Manual instantiate
            var go = Instantiate(prefab, parent);
            
            // Ensure alpha is 0 immediately
            var newCg = go.GetComponent<CanvasGroup>();
            if (newCg)
            {
                newCg.alpha = 0;
                newCg.blocksRaycasts = false;
            }
            
            return go;
        }

        /// <summary>
        /// Safely awaits multiple tasks using ListPool to avoid allocations and re-entrancy issues.
        /// </summary>
        protected async UniTask AwaitTasks(List<UniTask> tasks, bool awaitAnimation)
        {
            if (tasks == null || tasks.Count == 0)
            {
                if (tasks != null) ListPool<UniTask>.Release(tasks);
                return;
            }

            try
            {
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
            }
            finally
            {
                ListPool<UniTask>.Release(tasks);
            }
        }

        /// <summary>
        /// Closes a specific view belonging to this container.
        /// </summary>
        public abstract UniTask Close(UIView view, bool awaitAnimation = true);
    }

    public abstract class UIContainer<TView> : UIContainer where TView : UIView
    {
        private RectTransform _rectContainer;
        public override RectTransform RectContainer
        {
            get
            {
                if (_rectContainer == null) _rectContainer = GetComponent<RectTransform>();
                return _rectContainer;
            }
        }

        public override Transform TransformContainer => transform;
    }
}
