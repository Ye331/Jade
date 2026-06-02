using UnityEngine;

namespace Jade.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool DashHeld { get; private set; }

        private bool jumpPressedThisFrame;
        private bool jumpReleasedThisFrame;
        private bool dashPressedThisFrame;

        private void Update()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
            JumpHeld = Input.GetButton("Jump");
            DashHeld = Input.GetButton("Fire3");

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressedThisFrame = true;
            }

            if (Input.GetButtonUp("Jump"))
            {
                jumpReleasedThisFrame = true;
            }

            if (Input.GetButtonDown("Fire3"))
            {
                dashPressedThisFrame = true;
            }
        }

        public bool ConsumeJumpPressed()
        {
            if (!jumpPressedThisFrame)
            {
                return false;
            }

            jumpPressedThisFrame = false;
            return true;
        }

        public bool ConsumeJumpReleased()
        {
            if (!jumpReleasedThisFrame)
            {
                return false;
            }

            jumpReleasedThisFrame = false;
            return true;
        }

        public bool ConsumeDashPressed()
        {
            if (!dashPressedThisFrame)
            {
                return false;
            }

            dashPressedThisFrame = false;
            return true;
        }
    }
}
