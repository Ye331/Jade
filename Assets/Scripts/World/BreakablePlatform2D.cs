using UnityEngine;

namespace Jade.World
{
    public class BreakablePlatform2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer platformRenderer;
        [SerializeField] private Collider2D platformCollider;
        [SerializeField] private string platformName = "Breakable Platform";

        private bool broken;

        public bool IsBroken => broken;

        public void Configure(string displayName, SpriteRenderer renderer, Collider2D solidCollider)
        {
            platformName = displayName;
            platformRenderer = renderer;
            platformCollider = solidCollider;
        }

        private void Awake()
        {
            if (platformRenderer == null)
            {
                platformRenderer = GetComponent<SpriteRenderer>();
            }

            if (platformCollider == null)
            {
                platformCollider = GetComponent<Collider2D>();
            }
        }

        public void Break()
        {
            if (broken)
            {
                return;
            }

            broken = true;
            if (platformRenderer != null)
            {
                platformRenderer.enabled = false;
            }

            if (platformCollider != null)
            {
                platformCollider.enabled = false;
            }

            Debug.Log(platformName + " was broken by monster projectile.");
        }
    }
}
