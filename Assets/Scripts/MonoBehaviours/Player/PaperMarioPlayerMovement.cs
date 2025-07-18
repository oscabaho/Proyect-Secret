using UnityEngine;
using UnityEngine.InputSystem;
using ProyectSecret.MonoBehaviours.Player;

[RequireComponent(typeof(Rigidbody))]
public class PaperMarioPlayerMovement : MonoBehaviour
{
    // Evento para notificar cambio de inversión de cámara
    public event System.Action<bool> OnCameraInvertedChanged;
    
    [Header("Puntos de spawn de arma")]
    [SerializeField] private Transform WeaponPoint;
    [SerializeField] private Transform HitBoxPoint;

    // Estado de inversión de cámara
    private bool isCameraInverted = false;
    
    [Header("Configuración de Sprites")]
    [SerializeField] private Sprite spriteFrontal;
    [SerializeField] private Sprite spriteDerecha;
    [SerializeField] private Sprite spriteArribaDerecha;
    [SerializeField] private Sprite spriteArriba;
    [SerializeField] private Sprite spriteAbajo;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string dayActionMap = "PlayerDay";
    [SerializeField] private string nightActionMap = "PlayerNight";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputActionMap currentActionMap;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private SpriteRenderer spriteRenderer;
    private Camera activeCamera;

    // Backing field privado para el estado de movimiento hacia abajo
    private bool _isMovingDown;
    /// <summary>
    /// Indica si el jugador está presionando input de movimiento hacia abajo.
    /// Útil para el controlador de cámara y lógica de animación.
    /// </summary>
    public bool IsMovingDown {
        get => _isMovingDown;
        private set => _isMovingDown = value;
    }

    // Llamado por el controlador de cámara para notificar el estado de inversión
    public void SetCameraInverted(bool inverted)
    {
        isCameraInverted = inverted;
        OnCameraInvertedChanged?.Invoke(inverted);
        UpdateWeaponHolderDirection();
    }

    private void UpdateWeaponHolderDirection()
    {
        var equipmentController = GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
        if (equipmentController != null && equipmentController.EquipmentSlots != null)
        {
            System.Reflection.FieldInfo field = equipmentController.GetType().GetField("weaponHolder", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                Transform weaponHolder = field.GetValue(equipmentController) as Transform;
                if (weaponHolder != null)
                    weaponHolder.forward = isCameraInverted ? -transform.forward : transform.forward;
            }
        }
    }

    public void SetFrontalSprite()
    {
        if (spriteRenderer != null && spriteFrontal != null)
        {
            spriteRenderer.sprite = spriteFrontal;
            spriteRenderer.flipX = false;
        }
    }

    public Vector2 GetMoveInput()
    {
        if (moveAction != null)
            return moveAction.ReadValue<Vector2>();
        return Vector2.zero;
    }

    /// <summary>
    /// Asigna la cámara activa para el movimiento del jugador.
    /// Si no se asigna, busca en los hijos y luego en Camera.main.
    /// </summary>
    /// <param name="cam">Cámara a asignar</param>
    public void SetActiveCamera(Camera cam)
    {
        activeCamera = cam;
        SetDayInput();

        if (activeCamera == null)
        {
            activeCamera = GetComponentInChildren<Camera>();
            if (activeCamera == null)
            {
                Debug.LogError("PaperMarioPlayerMovement: No se encontró cámara activa");
            }
            else
            {
                Debug.LogWarning("PaperMarioPlayerMovement: Usando cámara en hijos");
            }
        }
    }

    /// <summary>
    /// Inicializa componentes y asigna cámara si es necesario.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        #if UNITY_EDITOR
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No se encontró SpriteRenderer");
        }
        #endif

        SubscribeToDayNightEvents();

        if (inputActions != null)
        {
            SetDayInput();
        }

        if (activeCamera == null)
        {
            activeCamera = GetComponentInChildren<Camera>();
            if (activeCamera == null && Camera.main != null)
            {
                activeCamera = Camera.main;
                Debug.LogWarning("PaperMarioPlayerMovement: Usando Camera.main");
            }
        }
    }

    private void SubscribeToDayNightEvents()
    {
        if (ProyectSecret.Events.GameEventBus.Instance != null)
        {
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<ProyectSecret.Events.DayStartedEvent>(OnDayStarted);
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<ProyectSecret.Events.NightStartedEvent>(OnNightStarted);
        }
    }

    private void OnDayStarted(ProyectSecret.Events.DayStartedEvent evt)
    {
        SetDayInput();
    }

    private void OnNightStarted(ProyectSecret.Events.NightStartedEvent evt)
    {
        SetNightInput();
    }

    public void SetDayInput()
    {
        SwitchActionMap(dayActionMap);
    }

    public void SetNightInput()
    {
        SwitchActionMap(nightActionMap);
    }

    private void SwitchActionMap(string actionMapName)
    {
        if (inputActions == null) return;
        
        if (currentActionMap != null)
        {
            currentActionMap.Disable();
        }
        
        currentActionMap = inputActions.FindActionMap(actionMapName);
        
        if (currentActionMap != null)
        {
            currentActionMap.Enable();
            moveAction = currentActionMap.FindAction(moveActionName);
            jumpAction = currentActionMap.FindAction(jumpActionName);
        }
        #if UNITY_EDITOR
        else
        {
            Debug.LogWarning($"No se encontró ActionMap: {actionMapName}");
        }
        #endif
    }

    void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    /// <summary>
    /// Actualiza el movimiento, sprites y salto del jugador cada frame.
    /// </summary>
    void Update()
    {
        if (moveAction != null && activeCamera != null)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            IsMovingDown = input.y < -0.1f;

            // Detecta si la cámara activa es la trasera (Camera) y aplica inversión de input
            var cameraController = GetComponent<PlayerCameraController>();
            bool invertInput = false;
            if (cameraController != null && cameraController.GetActiveCamera() != null)
            {
                // Si la cámara activa es la trasera, invierte el input
                invertInput = cameraController.GetActiveCamera().name == "Camera";
                cameraController.DetectarAcercamiento(IsMovingDown);
            }
            if (invertInput)
            {
                input.x = -input.x;
                input.y = -input.y;
            }

            Vector3 camForward = activeCamera.transform.forward;
            Vector3 camRight = activeCamera.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
            // En Unity, para Rigidbody se usa linearVelocity
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

            if (WeaponPoint != null && moveDir.sqrMagnitude > 0.01f)
                WeaponPoint.forward = moveDir;
            if (HitBoxPoint != null && moveDir.sqrMagnitude > 0.01f)
                HitBoxPoint.forward = moveDir;

            UpdateSprite(moveDir, camForward, camRight);
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void UpdateSprite(Vector3 moveDir, Vector3 camForward, Vector3 camRight)
    {
        if (spriteRenderer == null) return;

        float forwardDot = Vector3.Dot(moveDir, camForward);
        float rightDot = Vector3.Dot(moveDir, camRight);
        bool camInverted = isCameraInverted;

        // Detecta si la cámara activa es la trasera (Camera) y aplica inversión de sprites arriba/abajo y oblicuos
        var cameraController = GetComponent<PlayerCameraController>();
        bool invertUpDown = false;
        if (cameraController != null && cameraController.GetActiveCamera() != null)
        {
            invertUpDown = cameraController.GetActiveCamera().name == "Camera";
        }

        // Derecha
        if ((rightDot > 0.1f && Mathf.Abs(forwardDot) < 0.1f && !camInverted) ||
            (rightDot < -0.1f && Mathf.Abs(forwardDot) < 0.1f && camInverted))
        {
            spriteRenderer.sprite = spriteDerecha;
            spriteRenderer.flipX = false;
        }
        // Izquierda
        else if ((rightDot < -0.1f && Mathf.Abs(forwardDot) < 0.1f && !camInverted) ||
                 (rightDot > 0.1f && Mathf.Abs(forwardDot) < 0.1f && camInverted))
        {
            spriteRenderer.sprite = spriteDerecha;
            spriteRenderer.flipX = true;
        }
        // Arriba/Abajo (invertidos si la cámara es trasera)
        else if (!invertUpDown && forwardDot > 0.1f && Mathf.Abs(rightDot) < 0.1f)
        {
            spriteRenderer.sprite = spriteArriba;
            spriteRenderer.flipX = false;
        }
        else if (!invertUpDown && forwardDot < -0.1f && Mathf.Abs(rightDot) < 0.1f)
        {
            spriteRenderer.sprite = spriteAbajo;
            spriteRenderer.flipX = false;
        }
        else if (invertUpDown && forwardDot > 0.1f && Mathf.Abs(rightDot) < 0.1f)
        {
            spriteRenderer.sprite = spriteAbajo;
            spriteRenderer.flipX = false;
        }
        else if (invertUpDown && forwardDot < -0.1f && Mathf.Abs(rightDot) < 0.1f)
        {
            spriteRenderer.sprite = spriteArriba;
            spriteRenderer.flipX = false;
        }
        // Oblicuo arriba-derecha/abajo-derecha (invertidos si la cámara es trasera)
        else if (!invertUpDown && ((rightDot > 0.1f && forwardDot > 0.1f && !camInverted) ||
                                  (rightDot < -0.1f && forwardDot > 0.1f && camInverted)))
        {
            spriteRenderer.sprite = spriteArribaDerecha;
            spriteRenderer.flipX = false;
        }
        else if (!invertUpDown && ((rightDot < -0.1f && forwardDot > 0.1f && !camInverted) ||
                                   (rightDot > 0.1f && forwardDot > 0.1f && camInverted)))
        {
            spriteRenderer.sprite = spriteArribaDerecha;
            spriteRenderer.flipX = true;
        }
        else if (invertUpDown && ((rightDot > 0.1f && forwardDot < -0.1f && !camInverted) ||
                                  (rightDot < -0.1f && forwardDot < -0.1f && camInverted)))
        {
            spriteRenderer.sprite = spriteArribaDerecha;
            spriteRenderer.flipX = false;
        }
        else if (invertUpDown && ((rightDot < -0.1f && forwardDot < -0.1f && !camInverted) ||
                                   (rightDot > 0.1f && forwardDot < -0.1f && camInverted)))
        {
            spriteRenderer.sprite = spriteArribaDerecha;
            spriteRenderer.flipX = true;
        }

        // Rotación al soltar el input
        if (camInverted && moveDir.sqrMagnitude < 0.01f)
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}