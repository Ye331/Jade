using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class RespawnZone2D : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMotor2D player = other.GetComponent<PlayerMotor2D>();
            if (player != null)
            {
                player.RespawnAtSpawnPoint();
            }
        }
    }
}
