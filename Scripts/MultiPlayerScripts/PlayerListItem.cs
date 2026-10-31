using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public Text playerUserName;
    public Text teamText;

    Player player;

    int team;
    public void SetUp(Player _plyer,int _team)
    {
        player = _plyer;
        team = _team;
        playerUserName.text=_plyer.NickName;
        teamText.text="Team: "+_team;

        ExitGames.Client.Photon.Hashtable custumProps=new ExitGames.Client.Photon.Hashtable();
        custumProps["Team"] = _team;
        _plyer.SetCustomProperties(custumProps);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player==otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);

    }

}
