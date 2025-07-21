using UnityEngine;
using UnityEngine.InputSystem;
using ProyectSecret.MonoBehaviours.Player;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
public class PaperMarioPlayerMovement : MonoBehaviour
{
    // Evento para notificar cambio de inversión de cámara
    public event System.Action<bool> OnCameraInvertedChanged;
    
    // Estado de inversión de cámara
    public bool isCameraInverted = false;
    
    private Animator animator;

    private PlayerPointSwitcher pointSwitcher;
    [Header("Input System")]
    [field: SerializeField]
    public InputActionAsset InputActions { get; private set; }
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

    [Header("Sonidos")]
    [SerializeField] private AudioClip moveSound;
    private bool isMoving;

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

    // Fuente de audio dedicada para el sonido de movimiento en bucle.
    private AudioSource _movimientoAudioSource;

    /// <summary>
    /// Llamado por el controlador de cámara para notificar el estado de inversión.
    /// Sincroniza todos los puntos y objetos dependientes de la cámara.
    /// </summary>
    public void SetCameraInverted(bool inverted)
    {
        isCameraInverted = inverted;
        OnCameraInvertedChanged?.Invoke(inverted);        
        // Usamos la referencia cacheada. El '?' evita un error si el componente no existe.
        pointSwitcher?.UpdateActivePoints(isCameraInverted);
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
        animator = GetComponent<Animator>();
        pointSwitcher = GetComponent<PlayerPointSwitcher>();

        // Añadimos un AudioSource para el sonido de movimiento.
        _movimientoAudioSource = gameObject.AddComponent<AudioSource>();
        _movimientoAudioSource.playOnAwake = false;
        _movimientoAudioSource.loop = true; // El SoundManager también lo configura, pero es buena práctica.
        #if UNITY_EDITOR
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No se encontró SpriteRenderer");
        }
        #endif

        SubscribeToDayNightEvents();

        if (InputActions != null)
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
        if (InputActions == null) return;
        
        if (currentActionMap != null)
        {
            currentActionMap.Disable();
        }
        
        currentActionMap = InputActions.FindActionMap(actionMapName);
        
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

            // Si la cámara está invertida, el input de movimiento también se invierte.
            // La lógica ya no depende de buscar el controlador de cámara.
            if (isCameraInverted)
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
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

            // Usamos la referencia cacheada en lugar de buscar el componente cada frame.
            if (pointSwitcher != null && moveDir.sqrMagnitude > 0.01f)
            {
                if (!isCameraInverted)
                {
                    if (pointSwitcher.WeaponPoint != null)
                        pointSwitcher.WeaponPoint.forward = moveDir;
                    if (pointSwitcher.HitBoxPoint != null)
                        pointSwitcher.HitBoxPoint.forward = moveDir;
                }
                else
                {
                    if (pointSwitcher.WeaponPoint1 != null)
                        pointSwitcher.WeaponPoint1.forward = moveDir;
                    if (pointSwitcher.HitBoxPoint1 != null)
                        pointSwitcher.HitBoxPoint1.forward = moveDir;
                }
            }
            if(input.sqrMagnitude > 0.01f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            // --- Lógica de Sonido de Movimiento ---
            // Esto asegura que el sonido solo se inicie una vez y se detenga cuando el jugador pare.
            if (isMoving)
            {
                SoundManager.Instancia.IniciarEfectoEnLoop(moveSound, _movimientoAudioSource);
            }
            else
            {
                SoundManager.Instancia.DetenerEfectoEnLoop(_movimientoAudioSource);
            }

            // --- Animación ---
            if (animator != null)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
                animator.SetBool("IsMoving", input.sqrMagnitude > 0.01f);

                // FlipX para mirar a la derecha/izquierda según el input
                if (spriteRenderer != null)
                {
                    // Si el movimiento es hacia la derecha, flipX = false; izquierda, flipX = true
                    if (Mathf.Abs(input.x) > 0.1f)
                        spriteRenderer.flipX = input.x < 0f;
                }
            }
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
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
