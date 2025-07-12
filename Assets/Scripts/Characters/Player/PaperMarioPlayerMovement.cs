using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento lateral estilo Paper Mario usando el Nuevo Input System.
/// </summary>
 [RequireComponent(typeof(Rigidbody))]
 public class PaperMarioPlayerMovement : MonoBehaviour
 {
    // Estado de inversión de cámara
    private bool isCameraInverted = false;
    // Permite saber si el jugador está presionando input de movimiento
    public bool IsMovingDown { get; private set; } = false;

    // Llamado por el controlador de cámara para notificar el estado de inversión
    public void SetCameraInverted(bool inverted)
    {
        isCameraInverted = inverted;
        // Si la cámara se acaba de invertir, forzar la "dirección inicial" al soltar el input de atrás
        if (!inverted)
        {
            // Al restaurar la cámara, el input vuelve a la lógica normal automáticamente
            // Si quieres forzar la posición o animación, puedes hacerlo aquí
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
            Debug.LogWarning("No se encontró SpriteRenderer en el jugador. Añádelo al GameObject.");
        }
        SubscribeToDayNightEvents();
        if (inputActions != null)
        {
            SetDayInput(); // Por defecto inicia en modo día
        }
        else
        {
            Debug.LogWarning("InputActionAsset no asignado en el inspector.");
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
            Debug.LogWarning($"No se encontró el ActionMap '{actionMapName}' en el InputActionAsset.");
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
            // Detectar si el jugador está presionando hacia abajo (para la cámara)
            IsMovingDown = input.y < -0.1f;

            // Invertir el eje Z si la cámara está invertida
            float z = isCameraInverted ? -input.y : input.y;
            Vector3 move = new Vector3(input.x, 0, z); // Permite movimiento horizontal, vertical y diagonal
            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

            // Cambiar sprite según dirección (ajustar lógica si la cámara está invertida)
            if (spriteRenderer != null)
            {
                // Si la cámara está invertida, invertir la lógica de arriba/abajo
                float yDir = isCameraInverted ? -input.y : input.y;
                // Derecha
                if (input.x > 0.1f && Mathf.Abs(yDir) < 0.1f)
                {
                    spriteRenderer.sprite = spriteDerecha;
                    spriteRenderer.flipX = false;
                }
                // Izquierda
                else if (input.x < -0.1f && Mathf.Abs(yDir) < 0.1f)
                {
                    spriteRenderer.sprite = spriteDerecha;
                    spriteRenderer.flipX = true;
                }
                // Oblicuo arriba-derecha
                else if (input.x > 0.1f && yDir > 0.1f)
                {
                    spriteRenderer.sprite = spriteArribaDerecha;
                    spriteRenderer.flipX = false;
                }
                // Oblicuo arriba-izquierda
                else if (input.x < -0.1f && yDir > 0.1f)
                {
                    spriteRenderer.sprite = spriteArribaDerecha;
                    spriteRenderer.flipX = true;
                }
                // Arriba
                else if (Mathf.Abs(input.x) < 0.1f && yDir > 0.1f)
                {
                    spriteRenderer.sprite = spriteArriba;
                    spriteRenderer.flipX = false;
                }
                // Abajo
                else if (Mathf.Abs(input.x) < 0.1f && yDir < -0.1f)
                {
                    spriteRenderer.sprite = spriteAbajo;
                    spriteRenderer.flipX = false;
                }
            }

            // Restaurar input a valores iniciales al soltar el input de atrás tras invertir la cámara
            if (isCameraInverted && Mathf.Abs(input.x) < 0.1f && Mathf.Abs(input.y) < 0.1f)
            {
                // Cuando el jugador suelta el input después de invertir la cámara, restaurar la lógica de input
                // Esto solo afecta la lógica interna, la cámara permanece invertida
                isCameraInverted = false;
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
