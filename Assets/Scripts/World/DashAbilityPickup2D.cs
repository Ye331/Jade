using Jade.Player;
using Jade.UI;
using UnityEngine;

namespace Jade.World
{
    public class DashAbilityPickup2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;

        public void Configure(SpriteRenderer renderer)
        {
            visual = renderer;
            ConfigurePromptZone();
        }

        private void Awake()
        {
            ConfigurePromptZone();
        }

        private void ConfigurePromptZone()
        {
            AbilityPromptZone2D zone = GetComponent<AbilityPromptZone2D>();
            if (zone == null)
            {
                zone = gameObject.AddComponent<AbilityPromptZone2D>();
            }

            zone.Configure(
                "按 E 激活灵渠台",
                "提示：按 Shift 瞬间向前冲刺一段距离",
                UnlockDash,
                CanUnlockDash);
        }

        private void UnlockDash(PlayerAbilityInventory2D abilities)
        {
            abilities.UnlockDash();
            Debug.Log("Picked up ability: Air Dash");
        }

        private static bool CanUnlockDash(PlayerAbilityInventory2D abilities)
        {
            return abilities != null && !abilities.DashUnlocked;
        }
    }
}
