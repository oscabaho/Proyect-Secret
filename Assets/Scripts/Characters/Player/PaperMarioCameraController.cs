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
        if (playerMovementScript == null) return;

        // Detectar si el jugador mantiene input hacia abajo
        bool inputDown = playerMovementScript.IsMovingDown;

        if (!isCameraInverted)
        {
            if (inputDown)
            {
                moveTowardsCameraTimer += Time.deltaTime;
                if (moveTowardsCameraTimer >= invertThreshold)
                {
                    InvertCameraInstant();
                    moveTowardsCameraTimer = 0f;
                }
            }
            else
            {
                moveTowardsCameraTimer = 0f;
            }
        }
        // Ahora la cámara invertida permanece hasta que se invoque RestoreCameraInstant manualmente

        Vector3 desiredPosition = target.position + (isCameraInverted ? invertedOffset : offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // Método público para invertir la cámara instantáneamente
    public void InvertCameraInstant()
    {
        isCameraInverted = true;
        if (playerMovementScript != null)
        {
            playerMovementScript.SetCameraInverted(true);
            playerMovementScript.SetFrontalSprite();
        }
    }

    // Método público para restaurar la cámara instantáneamente
    public void RestoreCameraInstant()
    {
        isCameraInverted = false;
        if (playerMovementScript != null)
        {
            playerMovementScript.SetCameraInverted(false);
            playerMovementScript.SetDefaultSprite();
        }
    }
}
