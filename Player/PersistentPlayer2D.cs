using UnityEngine;

namespace Jade.Player
{
    [RequireComponent(typeof(PlayerMotor2D))]
    public class PersistentPlayer2D : MonoBehaviour
    {
        private static PersistentPlayer2D instance;

        public static PlayerMotor2D CurrentPlayer { get; private set; }
        public static bool HasInstance => instance != null;

        private void Awake()
        {
            instance = this;
            CurrentPlayer = GetComponent<PlayerMotor2D>();
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                return;
            }

            instance = null;
            CurrentPlayer = null;
        }
    }
}
