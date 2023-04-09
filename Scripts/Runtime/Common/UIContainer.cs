using System;
using UnityEngine;

namespace Pirex.UI.Base
{
    public class UIContainer : MonoBehaviour
    {
        private Transform transformContainer;
        private RectTransform rectContainer;

        public Transform TransformContainer => transformContainer;
        public RectTransform RectContainer => rectContainer;

        protected virtual void Awake()
        {
            transformContainer = this.transform;
            rectContainer = (RectTransform)this.transform;
        }

    }

}