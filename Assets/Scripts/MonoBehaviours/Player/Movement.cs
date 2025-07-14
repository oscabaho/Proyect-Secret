using UnityEngine;
using UnityEngine.InputSystem;

namespace ProyectSecret.Characters.Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float cameraFollowSpeed = 8f;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -6);

        private InputAction move;
        private InputAction look;
        private InputAction jump;
        private Vector2 movement;
        private Rigidbody rb;
        private Vector3 velocity;

        private void OnEnable()
        {
            inputActions.FindActionMap("Player").Enable();
        }

        private void OnDisable()
        {
            inputActions.FindActionMap("Player").Disable();
        }

        void Awake()
        {
            move = InputSystem.actions.FindAction("Move");
            look = InputSystem.actions.FindAction("Look");
            jump = InputSystem.actions.FindAction("Jump");
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            movement = move.ReadValue<Vector2>();
            PlayerMove();
            CameraFollow();
            if (jump.WasPressedThisFrame())
            {
                Jump();
            }
        }

        private void PlayerMove()
        {
            // Movimiento relativo a la cámara
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();
            Vector3 moveDir = camForward * movement.y + camRight * movement.x;
            moveDir.Normalize();
            Vector3 targetVelocity = moveDir * walkSpeed;
            velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * 10f);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);

            // Rotar personaje hacia la dirección de movimiento
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotateSpeed * Time.deltaTime));
            }
        }

        private void CameraFollow()
        {
            if (cameraTransform == null) return;
            Vector3 targetPos = transform.position + cameraOffset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, cameraFollowSpeed * Time.deltaTime);
            cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
        }

        private void Jump()
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}
