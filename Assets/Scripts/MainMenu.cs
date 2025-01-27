using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup[] canvasGroups;
    // [0] Main menu panel canvas group.
    // [1] Login panel canvas group.
    // [2] Signup panel canvas group.
    // [3] Error panel canvas group.

    // Buttons called from editor to switch menu panels.
    public void MainMenuButton()
    {
        HidePanels();
        ShowPanel(0);
    }

    public void LoginMenuButton()
    {
        HidePanels();
        ShowPanel(1);
    }

    public void SignupMenuButton()
    {
        HidePanels();
        ShowPanel(2);
    }

    // Error pop up panel will stand for 3 seconds.
    public IEnumerator ShowErrorPanel(string newErrorMessage)
    {
        ShowPanel(3);

        TMP_Text errorMessage = canvasGroups[3].GetComponentInChildren<TMP_Text>();
        errorMessage.SetText(newErrorMessage);

        yield return new WaitForSeconds(3f);
        HidePanel(3);
    }

    // Web pages.
    public void openPnPPage()
    {
        Application.OpenURL("https://github.com/OmerZeyveli/BookClub/blob/main/Privacy%26Policy.md");
    }
    
    public void openCreditsPage()
    {
        Application.OpenURL("https://github.com/OmerZeyveli/BookClub/tree/main?tab=readme-ov-file#credits");
    }


    // Hides
    void HidePanels()
    {
        for (int i = 0; i < canvasGroups.Length; i++)
        {
            canvasGroups[i].alpha = 0; // Fully transparent.
            canvasGroups[i].interactable = false; // Disable interaction.
            canvasGroups[i].blocksRaycasts = false; // Don't recieve input.
        }
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