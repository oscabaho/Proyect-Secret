using UnityEngine;

/// <summary>
/// Controla la cámara para que siga al jugador con un ángulo fijo, estilo Paper Mario.
/// </summary>
public class PaperMarioCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 invertedOffset = new Vector3(0, 5, 10);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float invertThreshold = 2f;
    private bool isCameraInverted;
    private float moveTowardsCameraTimer;
    private PaperMarioPlayerMovement playerMovementScript;


    void Start()
    {
        if (target == null) return;
        playerMovementScript = target.GetComponent<PaperMarioPlayerMovement>();
        if (playerMovementScript != null)
            playerMovementScript.OnCameraInvertedChanged += HandleCameraInvertedChanged;
    }

    // Maneja el evento de inversión de cámara
    private void HandleCameraInvertedChanged(bool inverted) { }

    void LateUpdate()
    {
        if (target == null || playerMovementScript == null) return;
        bool inputDown = playerMovementScript.IsMovingDown;
        moveTowardsCameraTimer = inputDown ? moveTowardsCameraTimer + Time.deltaTime : 0f;
        if (!isCameraInverted && moveTowardsCameraTimer >= invertThreshold)
        {
            InvertCameraInstant();
            moveTowardsCameraTimer = 0f;
        }
        Vector3 desiredPosition = target.position + (isCameraInverted ? invertedOffset : offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        // Si está invertida, mira al personaje en el eje Z; si no, mira normal
        if (isCameraInverted)
            transform.LookAt(target.position + target.forward * 1.5f);
        else
            transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // Método público para invertir la cámara instantáneamente
    public void InvertCameraInstant()
    {
        isCameraInverted = true;
        playerMovementScript?.SetCameraInverted(true);
        playerMovementScript?.SetFrontalSprite();
    }
}
