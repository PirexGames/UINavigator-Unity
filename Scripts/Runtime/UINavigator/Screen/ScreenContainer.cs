using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PirexGames.UI.Base
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();
        public static UIContainer Container;

        protected override void Awake()
        {
            base.Awake();
            Container = GetComponent<ScreenContainer>();
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
            var screenSpawn = await SpawnGO(keyAddressable, Container.TransformContainer);

            var screen = screenSpawn.GetComponent<Screen>();
            if (screen is null) return null;
            if (stackScreen.TryPeek(out var screenToHide))
                await screenToHide.OnPushExit(screen);

            await screen.OnPushEnter();
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
                Destroy(screen.gameObject);
            }
            await UniTask.Yield();
        }
        private void OnDestroy()
        {
            foreach (var item in stackScreen)
                Destroy(item.gameObject);
            stackScreen.Clear();
        }

    }
}
