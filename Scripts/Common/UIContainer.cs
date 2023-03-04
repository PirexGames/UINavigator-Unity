using UnityEngine;

namespace Pirex.UI.Base
{
    public class UIContainer : MonoBehaviour
    {
        [SerializeField] private GameObjectPoolSO gameObjectPoolSO;
        private Transform transformContainer;
        private RectTransform rectContainer;
        public Transform TransformContainer => transformContainer;
        public RectTransform RectContainer => rectContainer;
        public GameObjectPoolSO PoolSO => gameObjectPoolSO;
        protected virtual void Awake()
        {
            transformContainer = this.transform;
            rectContainer = (RectTransform)this.transform;
        }
    }
}