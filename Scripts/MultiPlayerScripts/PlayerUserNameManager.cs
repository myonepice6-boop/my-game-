using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUserNameManager : MonoBehaviour
{
    [SerializeField] private InputField usernameInput;
    [SerializeField] private Text errorMassageText;

    private void Start()
    {
        if(PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");

            PhotonNetwork.NickName = PlayerPrefs.GetString("username");

        }
    }
    public void plaayerUserNameInputValueChanged()
    {
        string userName=usernameInput.text;

        if (!string.IsNullOrEmpty(userName) && userName.Length <= 20)
        {
            PhotonNetwork.NickName=userName;
            PlayerPrefs.SetString("username", userName);
            errorMassageText.text = "";
            MenuManager.instance.OpenMenu("TitleMenu");


        }
        else
        {
            errorMassageText.text = "Username Must not Be Empty and Should be less than 20 Character.";
        }
    }
}
