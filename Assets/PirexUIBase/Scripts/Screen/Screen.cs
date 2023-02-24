using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class Screen : UIView
    {
        [SerializeField] protected bool isHidenOnPushOther = true;
        public void OnPushOtherScreen()
        {
            gameObject.SetActive(!isHidenOnPushOther);
        }
    }
}
