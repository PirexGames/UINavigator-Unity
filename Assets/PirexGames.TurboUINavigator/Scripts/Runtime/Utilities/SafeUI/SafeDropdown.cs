using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// A drop-in replacement for <see cref="UnityEngine.UI.Dropdown"/> with built-in
    /// spam-click protection via <see cref="UICooldownCore"/>.
    ///
    /// Classic mobile bug: tapping a dropdown rapidly spawns multiple overlay menus
    /// stacked on top of each other. This class stops that by absorbing the second
    /// tap entirely — no Show() is ever called a second time.
    /// </summary>
    [AddComponentMenu("PirexGames/Safe UI/Safe Dropdown")]
    public class SafeDropdown : Dropdown
    {
        [Header("Safe UI — Cooldown")]
        [Tooltip("Seconds this dropdown (and ALL other Safe UI elements) are blocked after opening.")]
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
        /// PRIMARY GATE — Unity's Dropdown calls <c>Show()</c> from OnPointerClick.
        /// Blocking this prevents the overlay menu from spawning on spam-tap.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnPointerClick(eventData);
        }

        /// <summary>
        /// Keyboard / gamepad submit — also opens the dropdown via <c>Show()</c>.
        /// </summary>
        public override void OnSubmit(BaseEventData eventData)
        {
            if (IsInCooldown()) return;
            UICooldownCore.TriggerGlobalCooldown(_cooldownDuration);
            base.OnSubmit(eventData);
        }
    }
}
