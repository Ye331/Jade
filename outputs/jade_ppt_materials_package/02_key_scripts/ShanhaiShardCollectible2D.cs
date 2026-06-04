using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class ShanhaiShardCollectible2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;

        public void Configure(SpriteRenderer renderer)
        {
            visual = renderer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerMotor2D>() == null)
            {
                return;
            }

            ShanhaiShardInventory2D inventory = other.GetComponent<ShanhaiShardInventory2D>();
            if (inventory == null)
            {
                inventory = other.gameObject.AddComponent<ShanhaiShardInventory2D>();
            }

            inventory.AddShard(gameObject.name);
            gameObject.SetActive(false);
        }
    }
}
