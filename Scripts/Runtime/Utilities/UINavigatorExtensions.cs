using Cysharp.Threading.Tasks;

namespace PirexGames.Core.UINavigator
{
    public static class UINavigatorExtensions
    {
        /// <summary>
        /// Closes the view and fire-and-forget the animation.
        /// </summary>
        public static void Pop(this UIView view, bool awaitAnimation = true)
        {
            if (view == null) return;
            view.Close(awaitAnimation).Forget();
        }

        /// <summary>
        /// Closes the view and awaits the animation completion.
        /// </summary>
        public static UniTask PopAsync(this UIView view, bool awaitAnimation = true)
        {
            if (view == null) return UniTask.CompletedTask;
            return view.Close(awaitAnimation);
        }
    }
}
