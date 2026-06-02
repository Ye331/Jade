using UnityEngine;
using System.Collections;

namespace Jade.World
{
    public class BreakablePlatform2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer platformRenderer;
        [SerializeField] private Collider2D platformCollider;
        [SerializeField] private string platformName = "Breakable Platform";
        [SerializeField] private Sprite[] breakAnimationFrames;
        [SerializeField] private Sprite brokenSprite;
        [SerializeField] private float breakFrameDuration = 0.12f;
        [SerializeField] private bool hideRendererAfterBreak = false;

        private bool broken;
        private Coroutine breakRoutine;

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
            if (platformCollider != null)
            {
                platformCollider.enabled = false;
            }

            if (breakRoutine != null)
            {
                StopCoroutine(breakRoutine);
            }

            breakRoutine = StartCoroutine(PlayBreakAnimation());
            Debug.Log(platformName + " was broken by monster projectile.");
        }

        private IEnumerator PlayBreakAnimation()
        {
            if (platformRenderer == null)
            {
                yield break;
            }

            platformRenderer.enabled = true;
            if (breakAnimationFrames != null)
            {
                for (int i = 0; i < breakAnimationFrames.Length; i++)
                {
                    if (breakAnimationFrames[i] == null)
                    {
                        continue;
                    }

                    platformRenderer.sprite = breakAnimationFrames[i];
                    yield return new WaitForSeconds(Mathf.Max(0.01f, breakFrameDuration));
                }
            }

            if (brokenSprite != null)
            {
                platformRenderer.sprite = brokenSprite;
                platformRenderer.enabled = true;
            }
            else
            {
                platformRenderer.enabled = !hideRendererAfterBreak;
            }

            if (hideRendererAfterBreak)
            {
                platformRenderer.enabled = false;
            }
        }
    }
}
