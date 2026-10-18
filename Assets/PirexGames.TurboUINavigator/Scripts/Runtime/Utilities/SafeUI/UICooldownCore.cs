namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// Zero-GC, O(1) brain for the Safe UI system.
    /// Tracks a single global "next valid time" so that any rapid click across
    /// ANY SafeButton / SafeToggle / SafeDropdown is absorbed by one central gate.
    /// No MonoBehaviour — pure static, no heap allocation ever.
    /// </summary>
    public static class UICooldownCore
    {
        /// <summary>Default cooldown in seconds applied when a UI element is triggered.</summary>
        public const float DEFAULT_COOLDOWN = 0.4f;

        // The earliest UnscaledTime at which the next interaction is allowed.
        private static float _globalNextValidTime = 0f;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if the system is still within a cooldown window and all
        /// incoming interactions should be silently discarded.
        /// </summary>
        public static bool IsGlobalInCooldown()
        {
            return UnityEngine.Time.unscaledTime < _globalNextValidTime;
        }

        /// <summary>
        /// Arms the global cooldown gate for <paramref name="duration"/> seconds
        /// starting from the current unscaled time.
        /// Call this AFTER a successful interaction (click / submit).
        /// </summary>
        /// <param name="duration">Seconds to block subsequent interactions.</param>
        public static void TriggerGlobalCooldown(float duration = DEFAULT_COOLDOWN)
        {
            _globalNextValidTime = UnityEngine.Time.unscaledTime + duration;
        }

        // ── Editor helpers ────────────────────────────────────────────────────

#if UNITY_EDITOR
        /// <summary>
        /// Resets state when entering / leaving Play Mode with Domain Reload disabled.
        /// </summary>
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload()
        {
            _globalNextValidTime = 0f;
        }
#endif
    }
}
