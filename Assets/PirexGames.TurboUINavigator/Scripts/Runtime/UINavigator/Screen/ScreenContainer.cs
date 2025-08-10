using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    public class ScreenContainer : UIContainer
    {
        private static Stack<Screen> stackScreen = new();
        internal static UIContainer Container;
        
        public static int Count => stackScreen.Count;

        protected override void Awake()
        {
            base.Awake();
            stackScreen = new();
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
            {
                if (screenToHide != null)
                    await screenToHide.OnPushExit(screen);
                else
                    stackScreen.Pop();
            }

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
                {
                    if (screenToShow != null)
                    {
                        if (!screenToShow.gameObject.activeSelf)
                        {
                            screenToShow.gameObject.SetActive(true);
                            await screenToShow.OnPopEnter();
                        }
                    } 
                    else
                    {
                        stackScreen.Pop();
                    }
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
