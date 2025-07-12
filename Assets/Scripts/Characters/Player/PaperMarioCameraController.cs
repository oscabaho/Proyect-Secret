using UnityEngine;

/// <summary>
/// Controla la cámara para que siga al jugador con un ángulo fijo, estilo Paper Mario.
/// </summary>
public class PaperMarioCameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Asigna el jugador aquí
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;

    private bool isCameraInverted = false;
    private float moveTowardsCameraTimer = 0f;
    private PaperMarioPlayerMovement playerMovementScript;
    [Header("Offset invertido (frontal)")]
    [SerializeField] private Vector3 invertedOffset = new Vector3(0, 5, 10);
    [SerializeField] private float invertThreshold = 2f;

    void Start()
    {
        if (target != null)
            playerMovementScript = target.GetComponent<PaperMarioPlayerMovement>();
    }

    void LateUpdate()
    {
        if (target == null) return;
        // Detectar si el jugador se mueve "hacia la cámara" (input vertical positivo)
        bool movingTowardsCamera = false;
        bool inputHeldTowardsCamera = false;
        if (playerMovementScript != null)
        {
            Vector2 input = playerMovementScript.GetMoveInput();
            // Detecta si el input de acercarse a la cámara está siendo sostenido
            inputHeldTowardsCamera = input.y < -0.1f;
            movingTowardsCamera = inputHeldTowardsCamera;
        }
        if (inputHeldTowardsCamera)
        {
            if (!isCameraInverted)
            {
                moveTowardsCameraTimer += Time.deltaTime;
                if (moveTowardsCameraTimer >= invertThreshold)
                {
                    isCameraInverted = true;
                    // Cambia el sprite del jugador a "frontal" si tienes uno
                    if (playerMovementScript != null)
                        playerMovementScript.SetFrontalSprite();
                }
            }
            // Si la cámara ya está invertida, no volver a ejecutar el cambio
        }
        else
        {
            moveTowardsCameraTimer = 0f;
            // Si la cámara está invertida y el input se suelta, restaurar la cámara y el sprite
            if (isCameraInverted)
            {
                isCameraInverted = false;
                if (playerMovementScript != null)
                    playerMovementScript.SetDefaultSprite();
            }
        }
        Vector3 desiredPosition = target.position + (isCameraInverted ? invertedOffset : offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
