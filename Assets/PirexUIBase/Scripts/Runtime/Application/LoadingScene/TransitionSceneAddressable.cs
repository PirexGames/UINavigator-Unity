using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using TMPro;

namespace PirexGames.UI.Base
{
    public class TransitionSceneAddressable : MonoBehaviour
    {
        [SerializeField] private DataSceneTransition dataSceneTransition;
        [SerializeField] private TextMeshProUGUI txtProcessing;

        private string keyScene;
        private UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> operationHandle;
        private void Awake()
        {
            keyScene = dataSceneTransition.GetKeyAddressable();
        }

        private void Start()
        {
            AsyncLoad();
        }

        private async void AsyncLoad()
        {
            operationHandle = Addressables.LoadSceneAsync(keyScene);
            while (!operationHandle.IsDone)
            {
                float progress = operationHandle.PercentComplete / 0.9f;
                await UniTask.Yield();
            }
        }
    }
}
