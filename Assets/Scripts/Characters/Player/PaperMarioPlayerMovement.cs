using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento lateral estilo Paper Mario usando el Nuevo Input System.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PaperMarioPlayerMovement : MonoBehaviour
{
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
            Vector3 move = new Vector3(input.x, 0, input.y); // Permite movimiento horizontal, vertical y diagonal
            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

            // Cambiar sprite según dirección
            if (spriteRenderer != null)
            {
                // Derecha
                if (input.x > 0.1f && Mathf.Abs(input.y) < 0.1f)
                {
                    spriteRenderer.sprite = spriteDerecha;
                    spriteRenderer.flipX = false;
                }
                // Izquierda
                else if (input.x < -0.1f && Mathf.Abs(input.y) < 0.1f)
                {
                    spriteRenderer.sprite = spriteDerecha;
                    spriteRenderer.flipX = true;
                }
                // Oblicuo arriba-derecha
                else if (input.x > 0.1f && input.y > 0.1f)
                {
                    spriteRenderer.sprite = spriteArribaDerecha;
                    spriteRenderer.flipX = false;
                }
                // Oblicuo arriba-izquierda
                else if (input.x < -0.1f && input.y > 0.1f)
                {
                    spriteRenderer.sprite = spriteArribaDerecha;
                    spriteRenderer.flipX = true;
                }
                // Arriba
                else if (Mathf.Abs(input.x) < 0.1f && input.y > 0.1f)
                {
                    spriteRenderer.sprite = spriteArriba;
                    spriteRenderer.flipX = false;
                }
                // Abajo
                else if (Mathf.Abs(input.x) < 0.1f && input.y < -0.1f)
                {
                    spriteRenderer.sprite = spriteAbajo;
                    spriteRenderer.flipX = false;
                }
                // Puedes agregar más condiciones para otras direcciones si tienes más sprites
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
