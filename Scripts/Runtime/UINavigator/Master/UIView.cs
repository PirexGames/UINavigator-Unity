using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace PirexGames.Core.UINavigator
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {

        protected CanvasGroup viewCG;
        protected bool isInitialized;
        public bool IsInitalized { get { return isInitialized; } }
        protected virtual void Awake()
        {
            viewCG = this.GetComponent<CanvasGroup>();
            viewCG.alpha = 0;
            isInitialized = false;
            Initialize().Forget();
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        protected async virtual UniTask Initialize()
        {
            isInitialized = true;
            await UniTask.CompletedTask;
        }
    }
}