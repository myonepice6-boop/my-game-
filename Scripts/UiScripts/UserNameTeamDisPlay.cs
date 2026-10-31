using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UserNameTeamDisPlay : MonoBehaviour
{
    public Text userNameText;

    public Text teamText;


    public PhotonView view;

    private void Start()
    {
        if(view.IsMine)
        {
            //dont want to display username
            gameObject.SetActive(false);
        }
        userNameText.text=view.Owner.NickName;
        //show team numper
        if(view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            teamText.text="Team "+team;
        }
    }
}
