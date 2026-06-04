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
        public const string DoubleJumpedParameter = "DoubleJumped";
        public const string LandedParameter = "Landed";
        public const string DashingParameter = "Dashing";

        [SerializeField] private Animator animator;
        [SerializeField] private float runAnimationMinSpeed = 0.85f;
        [SerializeField] private float runAnimationMaxSpeed = 1.15f;

        private static readonly int Speed01Hash = Animator.StringToHash(Speed01Parameter);
        private static readonly int VerticalSpeedHash = Animator.StringToHash(VerticalSpeedParameter);
        private static readonly int GroundedHash = Animator.StringToHash(GroundedParameter);
        private static readonly int JumpedHash = Animator.StringToHash(JumpedParameter);
        private static readonly int DoubleJumpedHash = Animator.StringToHash(DoubleJumpedParameter);
        private static readonly int LandedHash = Animator.StringToHash(LandedParameter);
        private static readonly int DashingHash = Animator.StringToHash(DashingParameter);
        private static readonly int RunStateHash = Animator.StringToHash("Run");

        private PlayerMotor2D motor;
        private bool hasSpeed01Parameter;
        private bool hasVerticalSpeedParameter;
        private bool hasGroundedParameter;
        private bool hasJumpedParameter;
        private bool hasDoubleJumpedParameter;
        private bool hasLandedParameter;
        private bool hasDashingParameter;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor2D>();
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            CacheAnimatorParameters();
        }

        private void Update()
        {
            if (animator == null || motor == null)
            {
                return;
            }

            CacheAnimatorParameters();

            SetFloatIfPresent(Speed01Hash, hasSpeed01Parameter, motor.Speed01);
            SetFloatIfPresent(VerticalSpeedHash, hasVerticalSpeedParameter, motor.VerticalSpeed);
            SetBoolIfPresent(GroundedHash, hasGroundedParameter, motor.IsGrounded);
            SetBoolIfPresent(DashingHash, hasDashingParameter, motor.IsDashing);

            if (motor.JumpedThisFrame && hasJumpedParameter)
            {
                animator.SetTrigger(JumpedHash);
            }

            if (motor.DoubleJumpedThisFrame && hasDoubleJumpedParameter && motor.ConsumeDoubleJumpedThisFrame())
            {
                animator.SetTrigger(DoubleJumpedHash);
            }

            if (motor.LandedThisFrame && hasLandedParameter)
            {
                animator.SetTrigger(LandedHash);
            }

            UpdateRunPlaybackSpeed();
        }

        private void CacheAnimatorParameters()
        {
            if (animator == null)
            {
                return;
            }

            hasSpeed01Parameter = HasParameter(Speed01Hash);
            hasVerticalSpeedParameter = HasParameter(VerticalSpeedHash);
            hasGroundedParameter = HasParameter(GroundedHash);
            hasJumpedParameter = HasParameter(JumpedHash);
            hasDoubleJumpedParameter = HasParameter(DoubleJumpedHash);
            hasLandedParameter = HasParameter(LandedHash);
            hasDashingParameter = HasParameter(DashingHash);
        }

        private bool HasParameter(int hash)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].nameHash == hash)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetFloatIfPresent(int hash, bool present, float value)
        {
            if (present)
            {
                animator.SetFloat(hash, value);
            }
        }

        private void SetBoolIfPresent(int hash, bool present, bool value)
        {
            if (present)
            {
                animator.SetBool(hash, value);
            }
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
