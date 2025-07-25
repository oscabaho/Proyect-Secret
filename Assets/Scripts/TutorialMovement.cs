using ProyectSecret.MonoBehaviours.Player;
using UnityEngine;
using UnityEngine.Windows;

public class TutorialMovement : MonoBehaviour
{
    private PlayerInputController _input; // Referencia al nuevo InputController

    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] TutorialCamera tutorialCamera;

    // Referencias a componentes
    private Rigidbody rb;

    // Propiedades públicas de estado para que otros componentes puedan leerlas
    public bool IsGrounded { get; private set; } = true;
    public Vector3 CurrentVelocity { get; private set; }

    void OnEnable()
    {
        if (_input != null) _input.OnJumpPressed += HandleJump;
    }

    void OnDisable()
    {
        if (_input != null) _input.OnJumpPressed -= HandleJump;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInputController>();
        tutorialCamera = GetComponentInChildren<TutorialCamera>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleGroundCheck();
    }

    private void HandleMovement()
    {
        if (_input != null)
        {
            Vector2 input = _input.MoveInput;

            //Movimiento del personaje
            rb.MovePosition(rb.position + transform.forward * input.y * moveSpeed * Time.deltaTime);
            rb.MovePosition(rb.position + transform.right * input.x * moveSpeed * Time.deltaTime);

            //Rotacion del personaje
            float rotationAmount = tutorialCamera.lookDirection.x * rotateSpeed * Time.deltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    private void HandleJump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleGroundCheck()
    {
        // Usamos un SphereCast para una detección de suelo más robusta que OnCollisionEnter.
        // Esto maneja mejor los bordes y las pendientes.
        float sphereRadius = 0.3f;
        float checkDistance = 0.1f; // Distancia desde la base del jugador
        Vector3 spherePosition = transform.position + Vector3.up * (sphereRadius - 0.05f);
        IsGrounded = Physics.CheckSphere(spherePosition, sphereRadius, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
    }
}
