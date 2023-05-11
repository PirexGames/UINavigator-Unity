using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pirex.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {

        protected CanvasGroup viewCG;
        protected bool isInitialized;
        protected virtual void Awake()
        {
            viewCG = this.GetComponent<CanvasGroup>();
            viewCG.alpha = 0;
            isInitialized = false;
        }

        private void Start()
        {
            Initialize().Forget();
        }

        protected async virtual UniTask Initialize()
        {
            isInitialized = true;
            await UniTask.CompletedTask;
        }
    }
}