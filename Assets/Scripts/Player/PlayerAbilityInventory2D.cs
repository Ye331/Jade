using UnityEngine;

namespace Jade.Player
{
    public class PlayerAbilityInventory2D : MonoBehaviour
    {
        [SerializeField] private bool dashUnlocked;

        public bool DashUnlocked => dashUnlocked;

        public void UnlockDash()
        {
            if (dashUnlocked)
            {
                return;
            }

            dashUnlocked = true;
            Debug.Log("Ability unlocked: Air Dash");
        }
    }
}
