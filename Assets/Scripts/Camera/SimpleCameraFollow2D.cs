using Jade.Player;
using UnityEngine;

namespace Jade.CameraTools
{
    public class SimpleCameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 offset = new Vector2(1.5f, 1f);
        [SerializeField] private float lookAheadDistance = 1.25f;
        [SerializeField] private float horizontalSmoothTime = 0.16f;
        [SerializeField] private float verticalSmoothTime = 0.28f;

        private float xVelocity;
        private float yVelocity;
        private PlayerMotor2D targetMotor;
        private float shakeTimer;
        private float shakeStrength;

        public void Configure(Transform followTarget)
        {
            target = followTarget;
            targetMotor = target != null ? target.GetComponent<PlayerMotor2D>() : null;
        }

        private void Awake()
        {
            if (target != null)
            {
                targetMotor = target.GetComponent<PlayerMotor2D>();
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            int facing = targetMotor != null ? targetMotor.FacingDirection : 1;
            Vector3 desired = target.position + new Vector3(offset.x + lookAheadDistance * facing, offset.y, transform.position.z - target.position.z);

            float x = Mathf.SmoothDamp(transform.position.x, desired.x, ref xVelocity, horizontalSmoothTime);
            float y = Mathf.SmoothDamp(transform.position.y, desired.y, ref yVelocity, verticalSmoothTime);
            Vector3 shakeOffset = Vector3.zero;
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                shakeOffset = new Vector3(
                    Random.Range(-shakeStrength, shakeStrength),
                    Random.Range(-shakeStrength, shakeStrength),
                    0f);
            }
            else
            {
                shakeStrength = 0f;
            }

            transform.position = new Vector3(x, y, transform.position.z) + shakeOffset;
        }

        public void Shake(float duration, float strength)
        {
            shakeTimer = Mathf.Max(shakeTimer, duration);
            shakeStrength = Mathf.Max(shakeStrength, strength);
        }
    }
}
