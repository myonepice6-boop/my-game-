using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class PlayerControlerManager : MonoBehaviourPunCallbacks
{
    PhotonView view;
    GameObject controler;

    public int playerTeam;
    private Dictionary<int , int> playerTeams = new Dictionary<int , int>();
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        if(view.IsMine)
        {
            createControler();
        }

    }

    void createControler()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("player's Team "+playerTeam);

        }
        AssignedPlayerToSpownErea(playerTeam);
    }

    void AssignedPlayerToSpownErea(int team)
    {
        GameObject spownErea1 = GameObject.Find("SpownErea1");
        GameObject spownErea2 = GameObject.Find("SpownErea2");


        if(spownErea1==null||spownErea2==null)
        {
            Debug.LogError("SpownErea not found");
            return;
        }
        Transform spownpoint = null;    
        if(team==1)
        {
            spownpoint=spownErea1.transform.GetChild(Random.Range(0,spownErea1.transform.childCount));
        }

        if (team == 2)
        {
            spownpoint = spownErea2.transform.GetChild(Random.Range(0, spownErea2.transform.childCount));
        }
        if(spownpoint!=null)
        {
            controler = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spownpoint.position,spownpoint.rotation, 0, new object[] { view.ViewID });
            Debug.Log("Instantiated player controler at spown point");

        }
        else
        {
            Debug.Log("error no spown point available for team " + team);
        }

    }

    void assignedTeamToAllPlayers()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if(player.CustomProperties.ContainsKey("Team"))
            {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber]=team;
                Debug.Log(PhotonNetwork.NickName+" is in "+team);

                AssignedPlayerToSpownErea(team);
            }
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controler);
        createControler();
    }

    public override void  OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        assignedTeamToAllPlayers();
    }
}
