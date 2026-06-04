using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class LevelGoal2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer markerRenderer;
        [SerializeField] private Color completedColor = new Color(1f, 0.9f, 0.35f, 1f);

        private bool completed;

        public void Configure(SpriteRenderer marker)
        {
            markerRenderer = marker;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (completed || other.GetComponent<PlayerMotor2D>() == null)
            {
                return;
            }

            completed = true;
            if (markerRenderer != null)
            {
                markerRenderer.color = completedColor;
            }

            Debug.Log("Graybox level complete. This is the current end of the prototype route.");
        }
    }
}
