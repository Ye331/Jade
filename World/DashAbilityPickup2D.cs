using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class DashAbilityPickup2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;

        public void Configure(SpriteRenderer renderer)
        {
            visual = renderer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerAbilityInventory2D abilities = other.GetComponent<PlayerAbilityInventory2D>();
            if (abilities == null)
                return;

            // 新增：获取玩家脚本、播放拾取技能音效
            PlayerMotor2D playerMotor = other.GetComponent<PlayerMotor2D>();
            if (playerMotor != null)
            {
                playerMotor.PlaySkillPickSound();
            }

            abilities.UnlockDash();
            Debug.Log("Picked up ability: Air Dash");
            gameObject.SetActive(false);
        }
    }
}