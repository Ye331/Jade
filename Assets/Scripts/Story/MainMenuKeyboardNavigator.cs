using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuKeyboardNavigator : MonoBehaviour
{
    [Header("从上到下排列的菜单按钮")]
    public Button[] menuButtons;

    [Header("默认选中的按钮下标")]
    public int defaultSelectedIndex = 0;

    private int currentIndex = 0;

    private void Start()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        currentIndex = Mathf.Clamp(defaultSelectedIndex, 0, menuButtons.Length - 1);

        EventSystem.current.SetSelectedGameObject(null); // 清空
        SelectButton(currentIndex);
    }

    private void Update()
    {
        if (menuButtons == null || menuButtons.Length == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmSelection();
        }
    }

    private void MoveSelection(int direction)
    {
        int newIndex = currentIndex + direction;

        // 不循环：到最上面就不能再往上，到最下面就不能再往下
        newIndex = Mathf.Clamp(newIndex, 0, menuButtons.Length - 1);

        if (newIndex == currentIndex)
        {
            return;
        }

        currentIndex = newIndex;
        SelectButton(currentIndex);
    }

    private void SelectButton(int index)
    {
        if (menuButtons[index] == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(menuButtons[index].gameObject);
    }

    private void ConfirmSelection()
    {
        if (currentIndex < 0 || currentIndex >= menuButtons.Length)
        {
            return;
        }

        Button currentButton = menuButtons[currentIndex];

        if (currentButton != null && currentButton.interactable)
        {
            currentButton.onClick.Invoke();
        }
    }
}