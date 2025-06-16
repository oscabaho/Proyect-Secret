using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField]private InputActionAsset inputActions;

    private InputAction move;
    private InputAction look;
    private InputAction jump;

    private Vector2 movement;
    private Vector2 lookDirection;

    [SerializeField]private Transform cameraTransform;
    private Rigidbody rb;
    
    [SerializeField]private float walkSpeed = 5;
    [SerializeField]private float rotateSpeed = 5;
    
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
        lookDirection = look.ReadValue<Vector2>();

        PlayerMove();

        if (jump.WasPressedThisFrame())
        {
            Jump();
        }
    }

    private void PlayerMove()
    {
        //Movimiento del personaje
        rb.MovePosition(rb.position + transform.forward * movement.y * walkSpeed * Time.deltaTime);
        rb.MovePosition(rb.position + transform.right * movement.x * walkSpeed * Time.deltaTime);

        //Rotacion del personaje
        float rotationAmount = lookDirection.x * rotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);

        //Rotacion vertical de la camara
        Vector3 cameraRotation = cameraTransform.eulerAngles;
        cameraRotation.x = cameraRotation.x - lookDirection.y * rotateSpeed * Time.deltaTime;
        if (cameraRotation.x > 80 && cameraRotation.x < 180)
        {cameraRotation.x = 80;}else if (cameraRotation.x < 280 && cameraRotation.x > 180) { cameraRotation.x = 280; }
        cameraTransform.eulerAngles = cameraRotation;
    }

    private void Jump()
    {
        rb.AddForceAtPosition(new Vector3(0,0.5f,0), Vector3.up, ForceMode.Impulse);
    }
}
