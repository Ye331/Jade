using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class Checkpoint2D : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private SpriteRenderer markerRenderer;
        [SerializeField] private Color inactiveColor = new Color(0.25f, 0.45f, 0.42f, 0.75f);
        [SerializeField] private Color activeColor = new Color(0.35f, 1f, 0.85f, 1f);

        private bool activated;

        private void Awake()
        {
            SetVisual(false);
        }

        public void Configure(Transform point, SpriteRenderer marker)
        {
            spawnPoint = point;
            markerRenderer = marker;
            SetVisual(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMotor2D player = other.GetComponent<PlayerMotor2D>();
            if (player == null)
            {
                return;
            }

            Vector3 point = spawnPoint != null ? spawnPoint.position : transform.position;
            player.SetSpawnPoint(point);

            if (!activated)
            {
                activated = true;
                SetVisual(true);
                Debug.Log("Checkpoint activated: " + gameObject.name);
            }
        }

        private void SetVisual(bool isActive)
        {
            if (markerRenderer != null)
            {
                markerRenderer.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
