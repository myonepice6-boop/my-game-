using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform playerTransform;

    public Transform cameraTransform;

    [Header("Camera Movement")]
    public Transform cameraPivot;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = 0.3f;

    public float cameraLookSpeed = 2f;
    public float cameraPivotSpeed = 2f;

    public float lookAgel;
    public float pivotAngle;

    public float minPivotAngle = -30;
    public float maxPivotAngle = 30;
    [Header("Camera collision")]
    public LayerMask collisionLayer;
    private float defoltPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadios = 0.2f;
    private Vector3 cameraVectorPosition;

    private PlayerMovement playerMovement;

    [Header("scope")]
    public GameObject ScopeCanves;
    private bool isScope=false;
    public GameObject playerUI;
    public Camera mainCamera;
    private float oraignalFOV = 60f;

    public GameManager gameManager
    {
        get
        {
            return FindFirstObjectByType<GameManager>();
        }
    }
    //Cursor
    private void Awake()
    {
        if (gameManager.isMenuOpen == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        inputManager = FindObjectOfType<InputManager>();
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();

        cameraTransform = Camera.main.transform;
        defoltPosition = cameraTransform.localPosition.z;
    }

    public void HandelAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        CameraCollision();
        IsPlayerScoped();
    }
     void FollowTarget()
    {
        Vector3 targetPosition=Vector3.SmoothDamp(transform.position,playerTransform.position,ref cameraFollowVelocity,cameraFollowSpeed);
        transform.position = targetPosition;
    }

     void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        lookAgel = lookAgel + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle=pivotAngle+(inputManager.cameraInputY * cameraPivotSpeed);

        pivotAngle=Mathf.Clamp(pivotAngle,minPivotAngle,maxPivotAngle);
         rotation = Vector3.zero;
        rotation.y = lookAgel;
         targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation=Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

        if(playerMovement.isMoving==false&& playerMovement.isSprinting==false)
        {
            playerTransform.rotation=Quaternion.Euler(0,lookAgel,0);

        }

    }

    void CameraCollision()
    {
        float targetPosition = defoltPosition;
        RaycastHit hit;
        Vector3 dirction = cameraTransform.position - cameraPivot.position;
        dirction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadios, dirction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance =Vector3.Distance(cameraPivot.position,hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }
        if(Mathf.Abs(targetPosition)<minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;

    }

    public void IsPlayerScoped()
    {
        if (gameManager.isMenuOpen == true) {
            return;
        }
        if(inputManager.scopeInput)
        {
            ScopeCanves.SetActive(true);
            playerUI.SetActive(false);
            mainCamera.fieldOfView = 20f;
        }
        else
        {
            ScopeCanves.SetActive(false);
            playerUI.SetActive(true);
            mainCamera.fieldOfView = oraignalFOV;
        }
    }
}
