namespace PirexGames.Core.UINavigator
{
    public static class UINavigator
    {
        public static bool AutoReleasePoolOnSceneChange { get; set; } = true;

        public static ScreenContainer Screens => GetSafe<ScreenContainer>();
        public static ModalContainer Modals => GetSafe<ModalContainer>();
        public static AlertContainer Alerts => GetSafe<AlertContainer>();
        public static TooltipContainer Tooltips => GetSafe<TooltipContainer>();
        public static ToastContainer MonoToasts => GetSafe<ToastContainer>("Default_Mono");
        public static ToastContainer MultipleToasts => GetSafe<ToastContainer>("Default_Multiple");

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (oldScene, newScene) =>
            {
                if (AutoReleasePoolOnSceneChange)
                {
                    UIContainer.ReleaseAllPools();
                }
            };
        }

        public static void ReleaseAllPools() => UIContainer.ReleaseAllPools();

        private static T GetSafe<T>(string name = "Default") where T : UIContainer
        {
            var container = UIContainer.Get<T>(name);
            return container;
        }

        public static T GetContainer<T>(string name = "Default") where T : UIContainer
        {
            return GetSafe<T>(name);
        }
    }
}
