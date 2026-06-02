using UnityEngine;

namespace Jade.Player
{
    public class PlayerAbilityInventory2D : MonoBehaviour
    {
        private static bool savedDashUnlocked;
        private static bool savedDoubleJumpUnlocked;
        private static bool savedWallJumpUnlocked;

        [SerializeField] private bool dashUnlocked;
        [SerializeField] private bool doubleJumpUnlocked;
        [SerializeField] private bool wallJumpUnlocked;

        public bool DashUnlocked => dashUnlocked;
        public bool DoubleJumpUnlocked => doubleJumpUnlocked;
        public bool WallJumpUnlocked => wallJumpUnlocked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSavedState()
        {
            savedDashUnlocked = false;
            savedDoubleJumpUnlocked = false;
            savedWallJumpUnlocked = false;
        }

        private void Awake()
        {
            dashUnlocked = dashUnlocked || savedDashUnlocked;
            doubleJumpUnlocked = doubleJumpUnlocked || savedDoubleJumpUnlocked;
            wallJumpUnlocked = wallJumpUnlocked || savedWallJumpUnlocked;
            SaveCurrentState();
        }

        public void UnlockDash()
        {
            if (dashUnlocked)
            {
                return;
            }

            dashUnlocked = true;
            savedDashUnlocked = true;
            Debug.Log("Ability unlocked: Air Dash");
        }

        public void UnlockDoubleJump()
        {
            if (doubleJumpUnlocked)
            {
                return;
            }

            doubleJumpUnlocked = true;
            savedDoubleJumpUnlocked = true;
            Debug.Log("Ability unlocked: Double Jump");
        }

        public void UnlockWallJump()
        {
            if (wallJumpUnlocked)
            {
                return;
            }

            wallJumpUnlocked = true;
            savedWallJumpUnlocked = true;
            Debug.Log("Ability unlocked: Wall Jump");
        }

        private void SaveCurrentState()
        {
            savedDashUnlocked = savedDashUnlocked || dashUnlocked;
            savedDoubleJumpUnlocked = savedDoubleJumpUnlocked || doubleJumpUnlocked;
            savedWallJumpUnlocked = savedWallJumpUnlocked || wallJumpUnlocked;
        }
    }
}
