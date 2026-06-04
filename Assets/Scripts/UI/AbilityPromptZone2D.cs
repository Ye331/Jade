using System;
using Jade.Player;
using UnityEngine;

namespace Jade.UI
{
    public class AbilityPromptZone2D : MonoBehaviour
    {
        [SerializeField] private string promptTitle = "按 E 激活灵渠台";
        [SerializeField] private string promptHint;

        private Action<PlayerAbilityInventory2D> activate;
        private Func<PlayerAbilityInventory2D, bool> canActivate;
        private PlayerAbilityInventory2D currentPlayer;
        private bool activated;

        public void Configure(string title, string hint, Action<PlayerAbilityInventory2D> activateAction, Func<PlayerAbilityInventory2D, bool> canActivatePredicate)
        {
            promptTitle = title;
            promptHint = hint;
            activate = activateAction;
            canActivate = canActivatePredicate;
        }

        private void Update()
        {
            if (activated || currentPlayer == null || !Input.GetKeyDown(KeyCode.E))
            {
                return;
            }

            if (canActivate != null && !canActivate(currentPlayer))
            {
                activated = true;
                HidePrompt();
                gameObject.SetActive(false);
                return;
            }

            if (activate != null)
            {
                activate(currentPlayer);
            }

            activated = true;
            HidePrompt();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (activated)
            {
                return;
            }

            PlayerAbilityInventory2D abilities = other.GetComponent<PlayerAbilityInventory2D>();
            if (abilities == null)
            {
                return;
            }

            if (canActivate != null && !canActivate(abilities))
            {
                activated = true;
                gameObject.SetActive(false);
                return;
            }

            currentPlayer = abilities;
            GameplayHud2D hud = GameplayHud2D.Active;
            if (hud != null)
            {
                hud.ShowAbilityPrompt(promptTitle, promptHint);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (currentPlayer == null || other.GetComponent<PlayerAbilityInventory2D>() != currentPlayer)
            {
                return;
            }

            currentPlayer = null;
            HidePrompt();
        }

        private void OnDisable()
        {
            currentPlayer = null;
            HidePrompt();
        }

        private static void HidePrompt()
        {
            GameplayHud2D hud = GameplayHud2D.Active;
            if (hud != null)
            {
                hud.HideAbilityPrompt();
            }
        }
    }
}
