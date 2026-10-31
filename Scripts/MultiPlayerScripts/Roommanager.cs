using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class Roommanager : MonoBehaviourPunCallbacks
{
    public static Roommanager Instance;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
                return;
        }
        DontDestroyOnLoad(gameObject);
        Instance= this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override  void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex==1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerControlerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
