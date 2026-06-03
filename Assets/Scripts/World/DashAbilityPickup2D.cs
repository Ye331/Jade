using Jade.Audio;
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
                "\u6309 E \u6fc0\u6d3b\u7075\u6e20\u53f0",
                "\u63d0\u793a\uff1a\u6309 Shift \u77ac\u95f4\u5411\u524d\u51b2\u523a\u4e00\u6bb5\u8ddd\u79bb",
                UnlockDash,
                CanUnlockDash);
        }

        private void UnlockDash(PlayerAbilityInventory2D abilities)
        {
            abilities.UnlockDash();
            GameAudio2D.PlayOneShot2D(GameAudio2D.SkillPickResourcePath, 0.75f);
            Debug.Log("Picked up ability: Air Dash");
        }

        private static bool CanUnlockDash(PlayerAbilityInventory2D abilities)
        {
            return abilities != null && !abilities.DashUnlocked;
        }
    }
}
