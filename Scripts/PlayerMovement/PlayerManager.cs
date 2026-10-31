using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerMovement playerMovement;

    CameraManager cameraManager;

    Animator animator;
    public bool isInterActing;

    PhotonView view;

    private void Awake()
    {
        view= GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraManager = GetComponent<CameraManager>();
        cameraManager = FindObjectOfType<CameraManager>();


    }
    //Cursor
    private void Start()
    {
        if(!view.IsMine)
        {
            Destroy(GetComponentInChildren<CameraManager>().gameObject);
        }
    }

    private void Update()
    {
        if (!view.IsMine)
            return;
        inputManager.HandelAllInputes();

    }

    private void FixedUpdate()
    {
        if (!view.IsMine)
            return;
        playerMovement.HandelAllMovement();
    }

    private void LateUpdate()
    {
        if (!view.IsMine)
            return;
        cameraManager.HandelAllCameraMovement();

        isInterActing = animator.GetBool("isIneracting");
        playerMovement.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerMovement.isGrounded);
    }
}
