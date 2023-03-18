using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();
        public static UIContainer Container;
        protected async override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
            foreach (Transform screenTf in Container.transform)
            {
                stackScreen.Push(screenTf.GetComponent<Screen>());
            }
            if (stackScreen.TryPeek(out var screen))
            {
                await screen.OnShow();
            }
        }

        public static async UniTask<Screen> Push(string keyAddressable)
        {
            var screenPrefab = await AddressableHelperT<GameObject>.GetAsync(keyAddressable);
            GameObject screenSpawn;
            if (Container.PoolSO)
                screenSpawn = Container.PoolSO.Spawn(screenPrefab, Container.TransformContainer);
            else
                screenSpawn = Instantiate(screenPrefab, Container.TransformContainer);
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
                if (Container.PoolSO)
                    Container.PoolSO.DeSpawn(screen.gameObject);
                else
                    Destroy(screen.gameObject);
            }
            await UniTask.Yield();
        }

    }
}
