using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePage : MonoBehaviour
{
    [SerializeField] CanvasGroup[] canvasGroups;
    // [0] Main menu panel canvas group.
    // [1] Login panel canvas group.
    // [2] Signup panel canvas group.
    // [3] Error panel canvas group.

    [SerializeField] RectTransform SelectionImage;


    // Buttons called from editor to switch menu panels.
    public void MonthlyButton()
    {
        HidePanel(1);
        ShowPanel(0);
        SelectionImage.anchoredPosition = new Vector2(-300, 0);
    }

    public void LibraryButton()
    {
        HidePanel(0);
        ShowPanel(1);
        SelectionImage.anchoredPosition = new Vector2(300, 0);
    }

    public void MenuButton()
    {
        ShowErrorPanel();
    }

    public void MenuYes()
    {
        SaveSystem.DeleteEmail();
        SceneManager.LoadScene(0);
    }

    public void MenuNo()
    {
        HidePanel(2);
    }

    public void ShowErrorPanel()
    {
        ShowPanel(2);
    }


    void HidePanel(int no)
    {
        canvasGroups[no].alpha = 0; // Fully transparent.
        canvasGroups[no].interactable = false; // Disable interaction.
        canvasGroups[no].blocksRaycasts = false; // Don't recieve input.
    }

    void ShowPanel(int no)
    {
        if (canvasGroups.Length < 1) { return; } // No group to show.

        canvasGroups[no].alpha = 1; // Fully visible.
        canvasGroups[no].interactable = true; // Enable interaction.
        canvasGroups[no].blocksRaycasts = true; // Recieve input.
    }
}