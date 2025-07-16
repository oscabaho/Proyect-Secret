using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento lateral estilo Paper Mario usando el Nuevo Input System.
/// </summary>
 [RequireComponent(typeof(Rigidbody))]
 public class PaperMarioPlayerMovement : MonoBehaviour
 {
    [Header("Puntos de spawn de arma")]
    [SerializeField] private Transform WeaponPoint;
    [SerializeField] private Transform HitBoxPoint;
    // Estado de inversión de cámara
    private bool isCameraInverted = false;
    // Permite saber si el jugador está presionando input de movimiento
    public bool IsMovingDown { get; private set; } = false;

    // Llamado por el controlador de cámara para notificar el estado de inversión
    public void SetCameraInverted(bool inverted)
    {
        isCameraInverted = inverted;
        // Actualiza el weaponHolder para que apunte al frente correcto
        var equipmentController = GetComponent<ProyectSecret.Inventory.PlayerEquipmentController>();
        if (equipmentController != null && equipmentController.EquipmentSlots != null)
        {
            Transform weaponHolder = equipmentController.GetType().GetField("weaponHolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(equipmentController) as Transform;
            if (weaponHolder != null)
            {
                // Si la cámara está invertida, apunta hacia atrás; si no, hacia adelante
                weaponHolder.forward = inverted ? -transform.forward : transform.forward;
            }
        }
    }
    [Header("Sprite frontal (mirando a la cámara)")]
    [SerializeField] private Sprite spriteFrontal;

    // Permite a otros scripts obtener el input de movimiento actual
    public Vector2 GetMoveInput()
    {
        if (moveAction != null)
            return moveAction.ReadValue<Vector2>();
        return Vector2.zero;
    }

    // Cambia el sprite a frontal
    public void SetFrontalSprite()
    {
        if (spriteRenderer != null && spriteFrontal != null)
        {
            spriteRenderer.sprite = spriteFrontal;
            spriteRenderer.flipX = false;
        }
    }

    // Restaura el sprite por defecto según la dirección
    public void SetDefaultSprite()
    {
        // Se fuerza a actualizar el sprite según el input actual
        Vector2 input = GetMoveInput();
        // Reutiliza la lógica de Update para cambiar el sprite
        if (Mathf.Abs(input.x) > 0.1f || Mathf.Abs(input.y) > 0.1f)
        {
            // Llama a la lógica de cambio de sprite (puedes extraerla a un método si prefieres)
            // Aquí se asume que el Update lo hará automáticamente en el siguiente frame
        }
    }
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions; // Asigna el InputActionAsset desde el inspector
    [SerializeField] private string dayActionMap = "PlayerDay";
    [SerializeField] private string nightActionMap = "PlayerNight";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputActionMap currentActionMap;
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded = true;

    [Header("Sprites de dirección")]
    [SerializeField] private Sprite spriteDerecha;
    [SerializeField] private Sprite spriteArribaDerecha;
    [SerializeField] private Sprite spriteArriba;
    [SerializeField] private Sprite spriteAbajo;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("No se encontró SpriteRenderer en el jugador. Añádelo al GameObject.");
            #endif
        }
        SubscribeToDayNightEvents();
        if (inputActions != null)
        {
            SetDayInput(); // Por defecto inicia en modo día
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogWarning("InputActionAsset no asignado en el inspector.");
            #endif
        }
    }

    private void SubscribeToDayNightEvents()
    {
        // Ejemplo usando GameEventBus, puedes adaptar a tu sistema de eventos
        ProyectSecret.Events.GameEventBus.Instance.Subscribe<ProyectSecret.Events.DayStartedEvent>(OnDayStarted);
        ProyectSecret.Events.GameEventBus.Instance.Subscribe<ProyectSecret.Events.NightStartedEvent>(OnNightStarted);
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
        if (currentActionMap != null) currentActionMap.Disable();
        currentActionMap = inputActions.FindActionMap(actionMapName);
        if (currentActionMap != null)
        {
            currentActionMap.Enable();
            moveAction = currentActionMap.FindAction(moveActionName);
            jumpAction = currentActionMap.FindAction(jumpActionName);
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogWarning($"No se encontró el ActionMap '{actionMapName}' en el InputActionAsset.");
            #endif
        }
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

    void Update()
    {
        if (moveAction != null)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            IsMovingDown = input.y < -0.1f;

            // Obtener la referencia a la cámara principal
            Camera cam = Camera.main;
            if (cam != null)
            {
                // Calcular la dirección de movimiento en el espacio de la cámara
                Vector3 camForward = cam.transform.forward;
                Vector3 camRight = cam.transform.right;
                camForward.y = 0f; // Solo movimiento en plano XZ
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();
                Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
                rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

                // Actualizar WeaponPoint y HitBoxPoint para que apunten en la dirección de movimiento
                if (WeaponPoint != null && moveDir.sqrMagnitude > 0.01f)
                    WeaponPoint.forward = moveDir;
                if (HitBoxPoint != null && moveDir.sqrMagnitude > 0.01f)
                    HitBoxPoint.forward = moveDir;

                // ...existing code for sprite selection...
                if (spriteRenderer != null)
                {
                    float forwardDot = Vector3.Dot(moveDir, camForward);
                    float rightDot = Vector3.Dot(moveDir, camRight);
                    bool camInverted = false;
                    if (Camera.main != null)
                    {
                        var camController = Camera.main.GetComponent<PaperMarioCameraController>();
                        if (camController != null)
                            camInverted = camController.IsCameraInverted();
                    }
                    // Derecha
                    if ((rightDot > 0.1f && Mathf.Abs(forwardDot) < 0.1f && !camInverted) || (rightDot < -0.1f && Mathf.Abs(forwardDot) < 0.1f && camInverted))
                    {
                        spriteRenderer.sprite = spriteDerecha;
                        spriteRenderer.flipX = false;
                    }
                    // Izquierda
                    else if ((rightDot < -0.1f && Mathf.Abs(forwardDot) < 0.1f && !camInverted) || (rightDot > 0.1f && Mathf.Abs(forwardDot) < 0.1f && camInverted))
                    {
                        spriteRenderer.sprite = spriteDerecha;
                        spriteRenderer.flipX = true;
                    }
                    // Arriba
                    else if (forwardDot > 0.1f && Mathf.Abs(rightDot) < 0.1f)
                    {
                        spriteRenderer.sprite = spriteArriba;
                        spriteRenderer.flipX = false;
                    }
                    // Abajo
                    else if (forwardDot < -0.1f && Mathf.Abs(rightDot) < 0.1f)
                    {
                        spriteRenderer.sprite = spriteAbajo;
                        spriteRenderer.flipX = false;
                    }
                    // Oblicuo arriba-derecha
                    else if (((rightDot > 0.1f && forwardDot > 0.1f && !camInverted) || (rightDot < -0.1f && forwardDot > 0.1f && camInverted)))
                    {
                        spriteRenderer.sprite = spriteArribaDerecha;
                        spriteRenderer.flipX = false;
            // Si la cámara está invertida y el input de movimiento se suelta, rota el personaje 180° en Y
            var camControllerCheck = Camera.main != null ? Camera.main.GetComponent<PaperMarioCameraController>() : null;
            if (camControllerCheck != null && camControllerCheck.IsCameraInverted() && input == Vector2.zero)
            {
                // Rota el personaje para que mire hacia la cámara
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
            }
                    }
                    // Oblicuo arriba-izquierda
                    else if (((rightDot < -0.1f && forwardDot > 0.1f && !camInverted) || (rightDot > 0.1f && forwardDot > 0.1f && camInverted)))
                    {
                        spriteRenderer.sprite = spriteArribaDerecha;
                        spriteRenderer.flipX = true;
                    }
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
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
    }
}
