using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.UI.Base
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDynamicScale : MonoBehaviour
    {
        [SerializeField] private Vector2 defaultScreenSize = new Vector2(1080, 1920);
        [SerializeField] private bool onAwake = false;
        [SerializeField] private bool onEnable = false;
        [SerializeField] private bool onStart = false;

        [SerializeField] private RectTransform thisRect;
        private Vector2 _originRectSize;

        private void Awake()
        {
            _originRectSize = thisRect.sizeDelta;
            if (!onAwake) return;
            Scale();
        }

        private void Start()
        {
            if (!onStart) return;
            Scale();
        }

        private void OnEnable()
        {
            if (!onEnable) return;
            Scale();
        }

        private void Scale()
        {
            var sizeScreen = new Vector2(UnityEngine.Screen.width, UnityEngine.Screen.height);
            var ratioHeight = sizeScreen.y / defaultScreenSize.y;
            var ratioWidth = sizeScreen.x / defaultScreenSize.x;
            if (Mathf.Abs(ratioHeight - 1) > Mathf.Abs(ratioWidth - 1))
                thisRect.sizeDelta = _originRectSize * ratioHeight;
            else
                thisRect.sizeDelta = _originRectSize * ratioWidth;
        }


#if UNITY_EDITOR
        private void Reset()
        {
            thisRect = GetComponent<RectTransform>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
