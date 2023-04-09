using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pirex.UI.Base
{
    public abstract class UIUpdateComp : MonoBehaviour
    {
        public abstract void OnUpdate();
        protected virtual void Awake()
        {
            UIUpdateSystem.Register(this);
        }

        protected virtual void OnDestroy()
        {
            UIUpdateSystem.UnRegister(this);
        }
    }
}
