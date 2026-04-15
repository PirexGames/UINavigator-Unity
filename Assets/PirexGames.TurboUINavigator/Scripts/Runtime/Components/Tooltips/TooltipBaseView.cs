using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class TooltipBaseView : EView
    {
        [SerializeField] protected TMPro.TextMeshProUGUI contentTxt;
        private Vector2 _lastPosition;
        private bool _lastIsLocal;

        internal virtual async UniTask Init(string content, Vector2 position, bool isLocalPosition = false)
        {
            if (contentTxt != null) contentTxt.text = content;
            _lastPosition = position;
            _lastIsLocal = isLocalPosition;
            await OnShow(position, isLocalPosition);
        }

        internal override async UniTask OnHide(Vector2? position = null, bool isLocalPosition = false)
        {
            if (position.HasValue)
                await base.OnHide(position, isLocalPosition);
            else
                await base.OnHide(_lastPosition, _lastIsLocal);
        }
    }
}
