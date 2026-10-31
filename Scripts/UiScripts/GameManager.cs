using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isMenuOpen = false;
    public GameObject menuUi;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)&& isMenuOpen==false)
        {

            menuUi.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isMenuOpen = true;
        }
        else if(Input.GetKeyUp(KeyCode.Escape) && isMenuOpen == true)
        {

            menuUi.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;

            isMenuOpen = false;
        }
    }

    public void leavGame()
    {
        Debug.Log("game leaved");
        SceneManager.LoadScene("Menu");
    }

    public void ClosePause()
    {
        menuUi.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isMenuOpen = false;
    }
}
