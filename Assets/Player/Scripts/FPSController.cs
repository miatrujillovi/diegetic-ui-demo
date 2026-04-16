using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    public Transform cameraPivot;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaDrain = 1f;
    public float staminaRecovery = 1.5f;

    public float currentStamina;
    private bool canSprint = true;

    [Header("Cámara")]
    public float standCameraHeight = 1.6f;
    public float crouchCameraHeight = 1.0f;
    public bool canLook = true;


    [Header("Estados")]
    public bool isHidingCloset = false;
    public bool isHidingTrunk = false;
    public bool isOnElevatedSurface = false;

    [Header("Interacción Modular")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public Transform raycastOrigin;




    private Animator animator;

    private CharacterController controller;
    private PlayerInput input;


    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;

        animator = GetComponentInChildren<Animator>();
        
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInput();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        input.Player.Jump.performed += ctx => Jump();

        input.Player.Sprint.performed += ctx => isSprinting = true;
        input.Player.Sprint.canceled += ctx => isSprinting = false;

        input.Player.Crouch.performed += ctx => ToggleCrouch();


        input.Player.Pause.performed += ctx => TogglePause();

        input.Player.Interact.performed += ctx => TryInteract();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        CheckGround();
        Move();
        Look();
        ApplyGravity();
 
        UpdateAnimations();
    }

    void CheckGround()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void Move()
    {
        bool isTryingToSprint = isSprinting && moveInput.y > 0 && canSprint && !isCrouching;

        float currentSpeed = isCrouching ? crouchSpeed : (isTryingToSprint ? sprintSpeed : speed);

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

      
    }
    void Look()
    {
        if (!canLook) return;

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
       
        if (isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            controller.height = crouchHeight;
            cameraPivot.localPosition = new Vector3(
                cameraPivot.localPosition.x,
                crouchCameraHeight,
                cameraPivot.localPosition.z
            );
        }
        else
        {
            controller.height = standHeight;
            cameraPivot.localPosition = new Vector3(
                cameraPivot.localPosition.x,
                standCameraHeight,
                cameraPivot.localPosition.z
            );
        }
    }

    void TryInteract()
    {

      
        
        // Si estamos pausados, no interactuar
        if (Time.timeScale == 0f) return;

        RaycastHit hit;
        // Lanzamos el rayo desde el pivote de la cámara hacia adelante
        if (Physics.Raycast(cameraPivot.position, cameraPivot.forward, out hit, interactionDistance, interactableLayer))
        {
            // Intentamos obtener el componente HideSpot que creamos antes
            HideSpot spot = hit.collider.GetComponent<HideSpot>();
            if (spot != null)
            {
                spot.Interactuar(this); // Nos pasamos a nosotros mismos como referencia
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (cameraPivot == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(cameraPivot.position, cameraPivot.forward * interactionDistance);
    }






// ANIMACIONES (BLEND TREE)
void UpdateAnimations()
    {
        float speedPercent = moveInput.magnitude;

        if (isCrouching)
            speedPercent *= 0.4f;
        else if (isSprinting)
            speedPercent *= 1f;
        else
            speedPercent *= 0.5f;

        //animator.SetFloat("Speed", speedPercent);
        //animator.SetBool("IsCrouching", isCrouching);
    }


    
    void TogglePause()
    {
        Debug.Log("PAUSA FUNCIONA");

        if (Time.timeScale == 0f)
            ResumeGame();
        else
            PauseGame();
    }

    public GameObject pauseUI;
    public GameObject playerUI;

    public void PauseGame()
    {
        canLook = false;

        pauseUI.SetActive(true);
        playerUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        canLook = true;
        pauseUI.SetActive(false);
        playerUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }
}