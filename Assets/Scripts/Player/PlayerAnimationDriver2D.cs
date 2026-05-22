using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationDriver2D : MonoBehaviour
    {
        public const string Speed01Parameter = "Speed01";
        public const string VerticalSpeedParameter = "VerticalSpeed";
        public const string GroundedParameter = "Grounded";
        public const string JumpedParameter = "Jumped";
        public const string LandedParameter = "Landed";
        public const string DashingParameter = "Dashing";

        [SerializeField] private Animator animator;
        [SerializeField] private float runAnimationMinSpeed = 0.85f;
        [SerializeField] private float runAnimationMaxSpeed = 1.15f;

        private static readonly int Speed01Hash = Animator.StringToHash(Speed01Parameter);
        private static readonly int VerticalSpeedHash = Animator.StringToHash(VerticalSpeedParameter);
        private static readonly int GroundedHash = Animator.StringToHash(GroundedParameter);
        private static readonly int JumpedHash = Animator.StringToHash(JumpedParameter);
        private static readonly int LandedHash = Animator.StringToHash(LandedParameter);
        private static readonly int DashingHash = Animator.StringToHash(DashingParameter);
        private static readonly int RunStateHash = Animator.StringToHash("Run");

        private PlayerMotor2D motor;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            if (animator == null || motor == null)
            {
                return;
            }

            animator.SetFloat(Speed01Hash, motor.Speed01);
            animator.SetFloat(VerticalSpeedHash, motor.VerticalSpeed);
            animator.SetBool(GroundedHash, motor.IsGrounded);
            animator.SetBool(DashingHash, motor.IsDashing);

            if (motor.JumpedThisFrame)
            {
                animator.SetTrigger(JumpedHash);
            }

            if (motor.LandedThisFrame)
            {
                animator.SetTrigger(LandedHash);
            }

            UpdateRunPlaybackSpeed();
        }

        private void UpdateRunPlaybackSpeed()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == RunStateHash)
            {
                animator.speed = Mathf.Lerp(runAnimationMinSpeed, runAnimationMaxSpeed, motor.Speed01);
            }
            else
            {
                animator.speed = 1f;
            }
        }
    }
}
