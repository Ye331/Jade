using Jade.Player;
using UnityEngine;

namespace Jade.World
{
    public class ShanhaiShardGate2D : MonoBehaviour
    {
        [SerializeField] private int requiredShards = 4;
        [SerializeField] private Collider2D solidCollider;
        [SerializeField] private Collider2D triggerCollider;
        [SerializeField] private SpriteRenderer gateRenderer;
        [SerializeField] private SpriteRenderer goalRenderer;
        [SerializeField] private Color lockedColor = new Color(0.55f, 0.18f, 0.18f, 0.9f);
        [SerializeField] private Color openColor = new Color(0.85f, 0.72f, 0.32f, 0.55f);

        private bool opened;
        private bool completed;

        public void Configure(int required, Collider2D solid, Collider2D trigger, SpriteRenderer gateVisual, SpriteRenderer goalVisual)
        {
            requiredShards = Mathf.Max(1, required);
            solidCollider = solid;
            triggerCollider = trigger;
            gateRenderer = gateVisual;
            goalRenderer = goalVisual;
            SetOpen(false);
        }

        private void Awake()
        {
            SetOpen(opened);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMotor2D player = other.GetComponent<PlayerMotor2D>();
            if (player == null)
            {
                return;
            }

            ShanhaiShardInventory2D inventory = other.GetComponent<ShanhaiShardInventory2D>();
            if (!opened)
            {
                if (inventory != null && inventory.CollectedShards >= requiredShards)
                {
                    SetOpen(true);
                    Debug.Log("Shanhai gate opened with " + inventory.CollectedShards + "/" + requiredShards + " shards.");
                }
                else
                {
                    int count = inventory != null ? inventory.CollectedShards : 0;
                    Debug.Log("Shanhai gate locked. Shards: " + count + "/" + requiredShards + ".");
                    return;
                }
            }

            if (completed)
            {
                return;
            }

            completed = true;
            if (goalRenderer != null)
            {
                goalRenderer.color = new Color(1f, 0.9f, 0.35f, 1f);
            }

            Debug.Log("Shanhai Gate graybox complete. Awaiting layout approval before art pass.");
        }

        private void SetOpen(bool isOpen)
        {
            opened = isOpen;
            if (solidCollider != null)
            {
                solidCollider.enabled = !isOpen;
            }

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }

            if (gateRenderer != null)
            {
                gateRenderer.color = isOpen ? openColor : lockedColor;
            }
        }
    }
}
