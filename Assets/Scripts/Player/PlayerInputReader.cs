using UnityEngine;

namespace Jade.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public bool JumpHeld { get; private set; }

        private bool jumpPressedThisFrame;
        private bool jumpReleasedThisFrame;

        private void Update()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
            JumpHeld = Input.GetButton("Jump");

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressedThisFrame = true;
            }

            if (Input.GetButtonUp("Jump"))
            {
                jumpReleasedThisFrame = true;
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
    }
}
