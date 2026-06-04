using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class Collectible2D : MonoBehaviour
    {
        private static int collectedCount;
        [SerializeField] private SpriteRenderer visual;

        public void Configure(SpriteRenderer renderer)
        {
            visual = renderer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMotor2D playerMotor = other.GetComponent<PlayerMotor2D>();
            if (playerMotor == null)
            {
                return;
            }

            //仅新增这一行金币音效
            playerMotor.PlayCoinPickSound();

            collectedCount++;
            Debug.Log("Collected jade shard: " + collectedCount);
            gameObject.SetActive(false);
        }
    }
}