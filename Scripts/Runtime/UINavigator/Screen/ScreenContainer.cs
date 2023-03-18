using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Pirex.UI.Base
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();
        public static UIContainer Container;
        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<UIContainer>();
        }

        private async void Start()
        {
            foreach (Transform screenTf in Container.transform)
            {
                var scr = screenTf.GetComponent<Screen>();
                if (scr)
                    stackScreen.Push(scr);
            }
            if (stackScreen.TryPeek(out var screen))
            {
                await screen.OnPushEnter();
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
            if (stackScreen.TryPeek(out var screenToHide))
                await screenToHide.OnPushExit();

            await screen.OnPushEnter();

            if (stackScreen.TryPeek(out var screenToHide1))
                screenToHide1.HideOnPushOther();

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
                    {
                        await screenToShow.OnPopEnter();
                        screenToShow.gameObject.SetActive(true);
                    }
                await screen.OnPopExit();
                if (Container.PoolSO)
                    Container.PoolSO.DeSpawn(screen.gameObject);
                else
                    Destroy(screen.gameObject);
            }
            await UniTask.Yield();
        }

    }
}
