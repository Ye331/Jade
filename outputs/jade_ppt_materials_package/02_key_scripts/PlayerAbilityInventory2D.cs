using UnityEngine;

namespace Jade.Player
{
    public class PlayerAbilityInventory2D : MonoBehaviour
    {
        private static bool savedDashUnlocked;
        private static bool savedDoubleJumpUnlocked;

        [SerializeField] private bool dashUnlocked;
        [SerializeField] private bool doubleJumpUnlocked;

        public bool DashUnlocked => dashUnlocked;
        public bool DoubleJumpUnlocked => doubleJumpUnlocked;
        public event System.Action AbilitiesChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSavedState()
        {
            savedDashUnlocked = false;
            savedDoubleJumpUnlocked = false;
        }

        private void Awake()
        {
            dashUnlocked = dashUnlocked || savedDashUnlocked;
            doubleJumpUnlocked = doubleJumpUnlocked || savedDoubleJumpUnlocked;
            SaveCurrentState();
            NotifyAbilitiesChanged();
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
            NotifyAbilitiesChanged();
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
            NotifyAbilitiesChanged();
        }

        private void SaveCurrentState()
        {
            savedDashUnlocked = savedDashUnlocked || dashUnlocked;
            savedDoubleJumpUnlocked = savedDoubleJumpUnlocked || doubleJumpUnlocked;
        }

        private void NotifyAbilitiesChanged()
        {
            if (AbilitiesChanged != null)
            {
                AbilitiesChanged();
            }
        }
    }
}
