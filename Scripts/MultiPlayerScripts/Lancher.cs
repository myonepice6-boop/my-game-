using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;
using System.Collections;
public class Lancher : MonoBehaviourPunCallbacks
{
    public static Lancher instance;
    [SerializeField] InputField roomNameInpuField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomlistItemPrefab;

    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerlistItemPrefab;
    public GameObject startButton;
    [Header("Restart Game Image")]
    public GameObject RestartGameImage;
    private bool isEnabled;

    int nextTeamNumber = 1;

    private void Awake()
    {
        instance=this;
    }

   
    private void Start()
    {
        Debug.Log("connicting to master ...");
        isEnabled = true;
        StartCoroutine(EnableRestartGameImage());
        PhotonNetwork.ConnectUsingSettings();
    }

    

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        if(isEnabled)
        {
            isEnabled=false;
            RestartGameImage.SetActive(isEnabled);
        }
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene=true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("UserNameMenu");
        Debug.Log("joined to lobby");
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInpuField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInpuField.text);
        MenuManager.instance.OpenMenu("LoadingMenu");
        
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;


        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {

            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            int teamNumber = GetNextTeamNumber();

            Instantiate(playerlistItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i],teamNumber);

        }
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnCreateRoomFailed(short returnCode, string EroorMessage)
    {
        errorText.text = "Room Generation UnSuccesful " + EroorMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");


    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("TitleMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomlistItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
            
        }
    }
    //Horizetal
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int teamNumber = GetNextTeamNumber();

        GameObject playerItem = Instantiate(playerlistItemPrefab, playerListContent);
        playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer,teamNumber);
    }

    private int GetNextTeamNumber()
    {
        int teamNumber = nextTeamNumber;
        nextTeamNumber = 3 - nextTeamNumber;
        return teamNumber;
    }

    public IEnumerator EnableRestartGameImage()
    {
        yield return new WaitForSeconds(10f);
        RestartGameImage.SetActive(isEnabled);
    }
}
