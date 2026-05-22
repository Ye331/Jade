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
            {
                return;
            }

            abilities.UnlockDash();
            Debug.Log("Picked up ability: Air Dash");
            gameObject.SetActive(false);
        }
    }
}
