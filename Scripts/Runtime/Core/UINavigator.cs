namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// The primary hub for all UI navigation operations in the project.
    /// Use this static class to control Screens, Modals, Toasts, and Tooltips globally.
    /// </summary>
    public static class UINavigator
    {
        /// <summary>
        /// If true, all object pools and Addressable asset caches will be cleared automatically 
        /// whenever a scene change occurs. Defaults to true.
        /// </summary>
        public static bool AutoReleasePoolOnSceneChange { get; set; } = true;

        /// <summary>
        /// Central container for managing the Stack of full-screen views.
        /// Use this for primary navigation.
        /// </summary>
        public static ScreenContainer Screens => GetSafe<ScreenContainer>();

        /// <summary>
        /// Central container for managing Modals and popups.
        /// Modals typically block underlying UI interaction.
        /// </summary>
        public static ModalContainer Modals => GetSafe<ModalContainer>();

        /// <summary>
        /// Specialized container for simplified Alert and Confirmation dialogs.
        /// </summary>
        public static AlertContainer Alerts => GetSafe<AlertContainer>();

        /// <summary>
        /// Central container for displaying smart, boundary-aware Tooltips.
        /// </summary>
        public static TooltipContainer Tooltips => GetSafe<TooltipContainer>();

        /// <summary>
        /// Toast container configured for "Mono" mode (one toast at a time).
        /// Reuses the "Default_Mono" registry key.
        /// </summary>
        public static ToastContainer MonoToasts => GetSafe<ToastContainer>("Default_Mono");

        /// <summary>
        /// Toast container configured for "Multiple" mode (can stack multiple toasts).
        /// Reuses the "Default_Multiple" registry key.
        /// </summary>
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

        /// <summary>
        /// Manually releases all object pools and destroys cached Addressable assets.
        /// Useful for manual memory management during high-pressure transitions.
        /// </summary>
        public static void ReleaseAllPools() => UIContainer.ReleaseAllPools();

        private static T GetSafe<T>(string name = "Default") where T : UIContainer
        {
            var container = UIContainer.Get<T>(name);
            return container;
        }

        /// <summary>
        /// Retrieves a registered UIContainer by type and name.
        /// </summary>
        public static T GetContainer<T>(string name = "Default") where T : UIContainer
        {
            return GetSafe<T>(name);
        }
    }
}
