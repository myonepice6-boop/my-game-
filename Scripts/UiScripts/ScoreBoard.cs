using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;
    public Text bloueTeamText;
    public Text redTeamText;
    public int bloueTeamScore=0;
    public int redTeamScore=0;

    PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        Instance=this;
    }

    public void PlayerDied(int playerTeam)
    {
        if(playerTeam==2)
        {
            bloueTeamScore++;
        }
        if (playerTeam == 1)
        {
            redTeamScore++;
        }

        view.RPC("UpdateScores", RpcTarget.All, bloueTeamScore, redTeamScore);
    }
    [PunRPC]
    public void UpdateScores(int bloueScore,int redScore)
    {
        bloueTeamScore = bloueScore;
        redTeamScore = redScore;

        bloueTeamText.text = bloueTeamScore.ToString();
        redTeamText.text=redTeamScore.ToString();
    }
}
