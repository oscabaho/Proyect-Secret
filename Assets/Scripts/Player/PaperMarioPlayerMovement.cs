using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento lateral estilo Paper Mario usando el Nuevo Input System.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PaperMarioPlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction; // Asigna la acción de movimiento (Vector2)
    [SerializeField] private InputActionReference jumpAction; // Asigna la acción de salto (Button)
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, 0); // Solo eje X (lateral)
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, 0);

        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
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
