using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace PirexGames.UI.Base
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
                _loadingOverlay.DOFade(1f, duration);
            }
            else
            {
                await _loadingOverlay.DOFade(0f, duration).AsyncWaitForCompletion();
                _loadingOverlay.gameObject.SetActive(false);
            }
        }
    }
}
