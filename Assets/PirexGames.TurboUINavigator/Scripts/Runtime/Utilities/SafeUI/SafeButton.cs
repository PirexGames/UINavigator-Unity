using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// A drop-in replacement for <see cref="UnityEngine.UI.Button"/> with built-in
    /// double-click and spam-click protection via <see cref="UICooldownCore"/>.
    ///
    /// Usage: Replace the Button component on any GameObject with SafeButton.
    /// All existing onClick listeners, transitions, and navigation settings are
    /// preserved — no other changes required.
    /// </summary>
    [AddComponentMenu("PirexGames/Safe UI/Safe Button")]
    public class SafeButton : Button
    {
        [Header("Safe UI — Cooldown")]
        [Tooltip("Seconds this button (and ALL other Safe UI elements) are blocked after a successful click.")]
        [SerializeField] private float _cooldownDuration = UICooldownCore.DEFAULT_COOLDOWN;

        // ── Cooldown gate (shared helper) ─────────────────────────────────────

        private bool IsInCooldown()
        {
            return UICooldownCore.IsGlobalInCooldown();
        }

        // ── Override interaction entry-points ─────────────────────────────────

        /// <summary>
        /// Block the visual pressed-state if we are mid-cooldown.
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsInCooldown()) return;
            base.OnPointerDown(eventData);
        }

        /// <summary>
        /// Block the visual released-state if we are mid-cooldown.
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsInCooldown()) return;
            base.OnPointerUp(eventData);
        }

        /// <summary>
        /// The primary gate: absorb the click and trigger cooldown on success.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnPointerClick(eventData);
        }

        /// <summary>
        /// Keyboard / gamepad submit path — also protected.
        /// </summary>
        public override void OnSubmit(BaseEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnSubmit(eventData);
        }
    }
}
