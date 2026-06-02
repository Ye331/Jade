using Jade.Player;
using Jade.UI;
using UnityEngine;

namespace Jade.World
{
    public class DoubleJumpAbilityPickup2D : MonoBehaviour
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
                "提示：连续两次空格可进行二段跳",
                UnlockDoubleJump,
                CanUnlockDoubleJump);
        }

        private void UnlockDoubleJump(PlayerAbilityInventory2D abilities)
        {
            abilities.UnlockDoubleJump();
            Debug.Log("Picked up ability: Double Jump");
        }

        private static bool CanUnlockDoubleJump(PlayerAbilityInventory2D abilities)
        {
            return abilities != null && !abilities.DoubleJumpUnlocked;
        }
    }
}
