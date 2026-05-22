using UnityEngine;

namespace Jade.Player
{
    public class PlayerAbilityInventory2D : MonoBehaviour
    {
        [SerializeField] private bool dashUnlocked;
        [SerializeField] private bool doubleJumpUnlocked;

        public bool DashUnlocked => dashUnlocked;
        public bool DoubleJumpUnlocked => doubleJumpUnlocked;

        public void UnlockDash()
        {
            if (dashUnlocked)
            {
                return;
            }

            dashUnlocked = true;
            Debug.Log("Ability unlocked: Air Dash");
        }

        public void UnlockDoubleJump()
        {
            if (doubleJumpUnlocked)
            {
                return;
            }

            doubleJumpUnlocked = true;
            Debug.Log("Ability unlocked: Double Jump");
        }
    }
}
