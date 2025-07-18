using UnityEngine;

/// <summary>
/// Controla la cámara para que siga al jugador con un ángulo fijo, estilo Paper Mario.
/// </summary>
public class PaperMarioCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Tooltip("Offset de la cámara en la vista normal (X = izquierda/derecha, Y = altura, Z = cercanía/lejanía).  Un valor mayor en Y dará un ángulo más 'desde arriba'.")]
    [SerializeField] private Vector3 offset = new Vector3(0, 7, -7);
    [Tooltip("Offset de la cámara en la vista invertida (X = izquierda/derecha, Y = altura, Z = cercanía/lejanía).  Un valor mayor en Y dará un ángulo más 'desde arriba'.")]
    [SerializeField] private Vector3 invertedOffset = new Vector3(0, 7, 7);

    [Tooltip("Ángulo de inclinación de la cámara en grados.  Valores positivos inclinan la cámara hacia abajo.")]
    [SerializeField] private float cameraAngleX = 30f;

    [SerializeField] private float invertThreshold = 2f;
    private bool isCameraInverted;
    private float moveTowardsCameraTimer;
    private PaperMarioPlayerMovement playerMovementScript;


    void Start()
    {
        if (target == null) return;
        playerMovementScript = target.GetComponent<PaperMarioPlayerMovement>();
        if (playerMovementScript != null)
        {
            playerMovementScript.OnCameraInvertedChanged += HandleCameraInvertedChanged;
            //playerMovementScript.SetActiveCamera(GetComponent<Camera>()); // Esto lo hará el ExplorationSceneInitializer
        }
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
        // Si la cámara es hijo del player, ajusta la posición y rotación correctamente
        if (isCameraInverted)
        {
            transform.localPosition = invertedOffset;
            transform.localRotation = Quaternion.Euler(cameraAngleX, 180f, 0f);
        }
        else
        {
            transform.localPosition = offset;   
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
