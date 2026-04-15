namespace PirexGames.Core.UINavigator
{
    public enum ViewPushMode
    {
        Standard,       // Push overlaying normally
        BringToFront,   // Find in stack, bring to top, else push new
        SetRoot         // Clear history, set this as new root
    }
}
