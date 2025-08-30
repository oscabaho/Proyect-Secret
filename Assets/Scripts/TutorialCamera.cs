using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialCamera : MonoBehaviour
{
    public Vector2 lookDirection;
    private InputAction look;

    [SerializeField] private float rotateSpeed = 5;
    [SerializeField] private Transform cameraTransform;
    void Awake()
    {
        look = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = GetComponent<Transform>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lookDirection = look.ReadValue<Vector2>();

        //Rotacion vertical de la camara
        Vector3 cameraRotation = cameraTransform.eulerAngles;
        cameraRotation.x = cameraRotation.x - lookDirection.y * rotateSpeed * Time.deltaTime;
        if (cameraRotation.x > 80 && cameraRotation.x < 180)
        { cameraRotation.x = 80; }
        else if (cameraRotation.x < 280 && cameraRotation.x > 180) { cameraRotation.x = 280; }
        cameraTransform.eulerAngles = cameraRotation;
    }
}
