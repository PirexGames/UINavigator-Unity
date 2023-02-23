using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ScreenContainer : MonoBehaviour
    {
        private static Stack<Screen> stackScreen = new();
        public static Transform screenContainer;
        public static RectTransform rectScreenContainer;

        private void Awake()
        {
            screenContainer = GameObject.FindObjectOfType<ScreenContainer>().transform;
            rectScreenContainer = screenContainer.GetComponent<RectTransform>();
        }

        public static async UniTask<Screen> Push(string keyAddressable)
        {
            var screenPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            var screenSpawn = Instantiate(screenPrefab, screenContainer);
            var screen = screenSpawn.GetComponent<Screen>();
            if (screen is null) return null;
            await screen.OnPush();

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
                await screen.OnPop();
                Destroy(screen.gameObject);
            }
            await UniTask.Yield();
        }

    }
}
