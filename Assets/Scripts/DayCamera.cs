using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DayCamera : MonoBehaviour
{
    private Vector2 lookDirection;
    private InputAction look;

    [SerializeField] private float rotateSpeed = 5;
    private int distanciaDeteccion = 10;

    Conversation conversation;
    void Awake()
    {
        look = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        lookDirection = look.ReadValue<Vector2>();
        float rotationAmount = lookDirection.x * rotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
        transform.rotation = transform.rotation * deltaRotation;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaDeteccion))
        {
            conversation = hit.collider.GetComponent<Conversation>();
            conversation.OnTriggerStay(hit.collider);
        }
        Debug.DrawRay(transform.position, transform.forward * distanciaDeteccion, Color.yellow);
    }
}
