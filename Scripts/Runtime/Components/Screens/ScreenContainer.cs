using Cysharp.Threading.Tasks;

namespace PirexGames.Core.UINavigator
{
    public class ScreenContainer : UIStackContainer<ScreenBaseView>
    {
        protected override async UniTask OnShowView(ScreenBaseView baseView)
        {
            baseView.Owner = this;
            baseView.ApplyPivotCenter();
            await baseView.OnPushEnter();
        }

        protected override async UniTask OnHideView(ScreenBaseView baseView)
        {
            await baseView.OnPopExit();
        }

        protected override async UniTask OnBeforeHideTop(ScreenBaseView currentTop, ScreenBaseView nextTop)
        {
            // Transition current top out because a new one is pushing in
            await currentTop.OnPushExit(nextTop);
        }

        protected override async UniTask OnBeforeShowNewTop(ScreenBaseView newTop, ScreenBaseView oldTop)
        {
            // Transition new top (which was below) in because the one above it popped
            await newTop.OnPopEnter();
        }
    }
}
