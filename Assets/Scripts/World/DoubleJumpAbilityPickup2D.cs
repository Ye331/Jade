using Jade.Audio;
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
                "\u6309 E \u6fc0\u6d3b\u7075\u6e20\u53f0",
                "\u63d0\u793a\uff1a\u8fde\u7eed\u4e24\u6b21\u7a7a\u683c\u53ef\u8fdb\u884c\u4e8c\u6bb5\u8df3",
                UnlockDoubleJump,
                CanUnlockDoubleJump);
        }

        private void UnlockDoubleJump(PlayerAbilityInventory2D abilities)
        {
            abilities.UnlockDoubleJump();
            GameAudio2D.PlayOneShot2D(GameAudio2D.SkillPickResourcePath, 0.75f);
            Debug.Log("Picked up ability: Double Jump");
        }

        private static bool CanUnlockDoubleJump(PlayerAbilityInventory2D abilities)
        {
            return abilities != null && !abilities.DoubleJumpUnlocked;
        }
    }
}
