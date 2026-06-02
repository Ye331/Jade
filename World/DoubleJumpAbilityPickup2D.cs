using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class DoubleJumpAbilityPickup2D : MonoBehaviour
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

            // 新增：播放技能拾取音效
            PlayerMotor2D playerMotor = other.GetComponent<PlayerMotor2D>();
            if (playerMotor != null)
            {
                playerMotor.PlaySkillPickSound();
            }

            abilities.UnlockDoubleJump();
            Debug.Log("Picked up ability: Double Jump");
            gameObject.SetActive(false);
        }
    }
}