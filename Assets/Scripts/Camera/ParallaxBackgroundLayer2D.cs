using UnityEngine;

namespace Jade.CameraTools
{
    [ExecuteAlways]
    public class ParallaxBackgroundLayer2D : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector2 parallaxFactor = new Vector2(0.2f, 0.08f);
        [SerializeField] private Vector2 driftSpeed = new Vector2(0.02f, 0f);
        [SerializeField] private float floatAmplitude = 0.05f;
        [SerializeField] private float floatSpeed = 0.25f;
        [SerializeField] private bool keepCenteredOnCamera = true;
        [SerializeField] private bool lockZ = true;

        private Vector3 initialPosition;
        private Vector3 initialCameraPosition;
        private bool initialized;

        private void OnEnable()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            if (!initialized)
            {
                Initialize();
            }

            if (cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    return;
                }

                cameraTransform = mainCamera.transform;
                Initialize();
            }

            Vector3 cameraDelta = cameraTransform.position - initialCameraPosition;
            float time = Application.isPlaying ? Time.time : 0f;
            Vector3 drift = new Vector3(driftSpeed.x * time, driftSpeed.y * time, 0f);
            Vector3 bob = new Vector3(0f, Mathf.Sin(time * floatSpeed) * floatAmplitude, 0f);
            Vector3 cameraFollow = keepCenteredOnCamera ? cameraDelta : Vector3.zero;
            Vector3 parallaxOffset = new Vector3(cameraDelta.x * parallaxFactor.x, cameraDelta.y * parallaxFactor.y, 0f);
            Vector3 target = initialPosition + cameraFollow + parallaxOffset + drift + bob;
            if (lockZ)
            {
                target.z = initialPosition.z;
            }

            transform.position = target;
        }

        public void Configure(Transform followCamera, Vector2 parallax, Vector2 drift, float amplitude, float speed)
        {
            cameraTransform = followCamera;
            parallaxFactor = parallax;
            driftSpeed = drift;
            floatAmplitude = amplitude;
            floatSpeed = speed;
            Initialize();
        }

        private void Initialize()
        {
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            initialPosition = transform.position;
            initialCameraPosition = cameraTransform != null ? cameraTransform.position : Vector3.zero;
            initialized = true;
        }
    }
}
