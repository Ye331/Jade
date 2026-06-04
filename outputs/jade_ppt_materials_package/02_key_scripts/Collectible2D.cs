using Jade.Player;
using Jade.Audio;
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
            if (other.GetComponent<PlayerMotor2D>() == null)
            {
                return;
            }

            collectedCount++;
            GameAudio2D.PlayOneShot2D(GameAudio2D.CoinPickResourcePath, 0.7f);
            Debug.Log("Collected jade shard: " + collectedCount);
            gameObject.SetActive(false);
        }
    }
}
