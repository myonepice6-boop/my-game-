using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [Header("player Helth")]
    const float maxHealth = 150f;
    public float currentHelth;
    public Slider healthBarSlider;
    public GameObject playerUI;

    [Header("Ref & phsycs")]
  InputManager inputManager;
    PlayerManager playerManager;
    PlayerControlerManager playerControlerManager;
    AnimatorManager animatorManager;
    Vector3 moveDirecton;
    Transform cameraGameOpject;
    Rigidbody playerRigidbody;

    [Header("Falling and Landing")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHightOffset = 0.5f;
    public LayerMask groundLayer;
    [Header("Movement floags")]
    public bool isMoving;
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Valus")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 13f;

    public float sprintingSpeed = 7f;

    [Header("Jump var")]
    public float jumpHight = 4f;
    public float gravityIntensity = -15f;

    PhotonView view;

    public int playerTeam;


    private void Awake()
    {
        currentHelth = maxHealth;
        
        inputManager =GetComponent<InputManager>();
        playerManager=GetComponent<PlayerManager>();
        animatorManager=GetComponent<AnimatorManager>();
        playerRigidbody=GetComponent<Rigidbody>();

        cameraGameOpject =Camera.main.transform;
view = GetComponent<PhotonView>();
        playerControlerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControlerManager>();
        healthBarSlider.minValue = 0f;

        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHelth;
    
    }
    private void Start()
    {
        if(!view.IsMine)
        {
            Destroy(playerRigidbody);
            Destroy(playerUI);
        }

        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }
    public void HandelAllMovement()
    {
        HandelFallingAndLanding();
        if (playerManager.isInterActing)
        {
            return;
        }
        HandleMovement();
        HandelRotation();
    }

    public void HandleMovement()
    {
        if (isJumping) return;

        moveDirecton = new Vector3(cameraGameOpject.forward.x, 0f, cameraGameOpject.forward.z) * inputManager.vertecalInput;
        moveDirecton = moveDirecton + cameraGameOpject.right * inputManager.horizontalInput;
        moveDirecton.Normalize();

        moveDirecton.y = 0;

        if (isSprinting)
        {
            moveDirecton = moveDirecton * sprintingSpeed;

        }
        else
        {
            if (inputManager.movementAmount >= 0.5f)
            {
                moveDirecton = moveDirecton * movementSpeed;
                isMoving = true;
            }
            if (inputManager.movementAmount <= 0f)
            {
                isMoving = false;
            }
        }
        Vector3 movementVelocetiy = moveDirecton;
        playerRigidbody.linearVelocity = movementVelocetiy;
    }
    //Cursor
    public void HandelRotation()
    {
        if (isJumping) return;

        Vector3 targetDirection = Vector3.zero;
        targetDirection=cameraGameOpject.forward*inputManager.vertecalInput;
        targetDirection=targetDirection+ cameraGameOpject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        if(targetDirection==Vector3.zero)
        {
            targetDirection=transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    void HandelFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHightOffset;
        targetPosition = transform.position;

        if(!isGrounded &&! isJumping)
        { 
            if(!playerManager.isInterActing)
            {
                animatorManager.PlayTargetAnim("Falling", true);
            }
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if(Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if(!isGrounded && !playerManager.isInterActing)
            {
                animatorManager.PlayTargetAnim("Landing",true);
            }

            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded=false;
        }
        if (isGrounded && !isJumping) 
        {
            if (playerManager.isInterActing|| inputManager.movementAmount>0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);

            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandelJumping()
    {
        if(isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump",false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHight);
            Vector3 playerVelocety = moveDirecton;
            playerVelocety.y = jumpingVelocity;
            playerRigidbody.linearVelocity=playerVelocety;

            isJumping = false;
        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

    public void ApplyDamage(float damageValue)
    {
        view.RPC("RPC_takeDamage", RpcTarget.All, damageValue);
    }
    [PunRPC]
    void RPC_takeDamage(float damage)
    {
        if (!view.IsMine)
        {
            return;
            
        }
        currentHelth -= damage;
        healthBarSlider.value=currentHelth;
        if(currentHelth <= 0)
        {
            Die();

        }

        Debug.Log("damage taken " + damage);
        Debug.Log("current Health " + currentHelth);

    }

    private void Die()
    {
        playerControlerManager.Die();
        ScoreBoard.Instance.PlayerDied(playerTeam);

    }

}
