using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();

        protected override void Awake()
        {
            transformContainer = GameObject.FindObjectOfType<UIContainer>().transform;
            rectContainer = transformContainer.GetComponent<RectTransform>();
        }

        public static async UniTask<Screen> Push(string keyAddressable)
        {
            var screenPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            var screenSpawn = Instantiate(screenPrefab, transformContainer);
            var screen = screenSpawn.GetComponent<Screen>();
            if (screen is null) return null;
            await screen.OnShow();

            if (stackScreen.TryPeek(out var screenToHide))
                screenToHide.OnPushOtherScreen();
            stackScreen.Push(screen);

            await UniTask.Yield();
            return screen;
        }

        public static async UniTask Pop()
        {
            if (stackScreen.TryPeek(out var screen))
            {
                stackScreen.Pop();

                if (stackScreen.TryPeek(out var screenToShow))
                    if (!screenToShow.gameObject.activeSelf)
                        screenToShow.gameObject.SetActive(true);
                await screen.OnHide();
                Destroy(screen.gameObject);
            }
            await UniTask.Yield();
        }

    }
}
