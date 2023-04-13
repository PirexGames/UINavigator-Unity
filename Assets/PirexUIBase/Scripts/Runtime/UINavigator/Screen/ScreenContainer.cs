using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Pirex.UI.Base
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();
        public static UIContainer Container;

        private static float timeIntervalPush = .25f;
        protected static float currentTime;
        protected override void Awake()
        {
            base.Awake();
            currentTime = -timeIntervalPush * 2;
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
            if (!ValidPush()) return null;
            var screenSpawn = await SpawnGO(keyAddressable, Container.TransformContainer);

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
            currentTime = -timeIntervalPush * 2;
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
        protected static bool ValidPush()
        {
            if (Time.fixedTime - currentTime >= timeIntervalPush)
            {
                currentTime = Time.fixedTime;
                return true;
            }
            return false;
        }
    }
}
