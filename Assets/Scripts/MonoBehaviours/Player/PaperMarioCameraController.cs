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
        // Si la cámara es hijo del player, solo ajusta el offset local y la rotación local
        transform.localPosition = isCameraInverted ? invertedOffset : offset;
        if (isCameraInverted)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }
    }

    // Método público para invertir la cámara instantáneamente
    public void InvertCameraInstant()
    {
        isCameraInverted = true;
        playerMovementScript?.SetCameraInverted(true);
        playerMovementScript?.SetFrontalSprite();
    }
}
