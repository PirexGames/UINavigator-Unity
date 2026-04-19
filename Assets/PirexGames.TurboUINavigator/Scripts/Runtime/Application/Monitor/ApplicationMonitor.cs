using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif
using Cysharp.Threading.Tasks;

namespace PirexGames.Core.UINavigator
{
    public class ApplicationMonitor : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingOverlay;
        [SerializeField] private TextMeshProUGUI txtContentLoading;

        private static CanvasGroup _loadingOverlay;
        private static TextMeshProUGUI _txtContentLoading;


        private const float duration = .16f;
        private void Awake()
        {
            _loadingOverlay = loadingOverlay;
            _txtContentLoading = txtContentLoading;

            _loadingOverlay.gameObject.SetActive(false);
            _loadingOverlay.alpha = 0f;
        }

        public static async void Processing(bool process, string contentProcess = null)
        {
            if (_txtContentLoading) _txtContentLoading.text = contentProcess;
            if (process)
            {
                _loadingOverlay.gameObject.SetActive(true);
#if DOTWEEN_ENABLED
                _loadingOverlay.DOFade(1f, duration).SetUpdate(true);
#else
                _loadingOverlay.alpha = 1f;
#endif
            }
            else
            {
#if DOTWEEN_ENABLED
                await _loadingOverlay.DOFade(0f, duration).SetUpdate(true).AsyncWaitForCompletion();
#else
                _loadingOverlay.alpha = 0f;
                await UniTask.CompletedTask;
#endif
                _loadingOverlay.gameObject.SetActive(false);
            }
        }
    }
}
