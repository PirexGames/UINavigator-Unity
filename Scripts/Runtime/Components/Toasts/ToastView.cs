using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class ToastView : EView
    {
        [SerializeField] protected TMPro.TextMeshProUGUI contentTxt;

        public virtual async UniTask Init(string content)
        {
            if (contentTxt != null) contentTxt.text = content;
            await OnShow();
        }
    }
}
