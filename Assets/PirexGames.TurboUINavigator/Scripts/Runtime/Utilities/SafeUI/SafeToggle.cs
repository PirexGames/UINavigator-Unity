using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// A drop-in replacement for <see cref="UnityEngine.UI.Toggle"/> with built-in
    /// double-click and spam-click protection via <see cref="UICooldownCore"/>.
    ///
    /// Key detail: Unity flips <c>isOn</c> inside <c>OnPointerClick</c>, so that is
    /// the most critical gate to block during cooldown.
    /// </summary>
    [AddComponentMenu("PirexGames/Safe UI/Safe Toggle")]
    public class SafeToggle : Toggle
    {
        [Header("Safe UI — Cooldown")]
        [Tooltip("Seconds this toggle (and ALL other Safe UI elements) are blocked after a successful interaction.")]
        [SerializeField] private float _cooldownDuration = UICooldownCore.DEFAULT_COOLDOWN;

        // ── Cooldown gate ─────────────────────────────────────────────────────

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
        /// PRIMARY GATE — Unity calls <c>Set()</c> (which flips isOn) from here.
        /// Must be blocked during cooldown to prevent state toggling on spam.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnPointerClick(eventData);
        }

        /// <summary>
        /// Keyboard / gamepad submit — also flips isOn via <c>Set()</c>.
        /// </summary>
        public override void OnSubmit(BaseEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnSubmit(eventData);
        }
    }
}
