using UnityEngine;

/// <summary>
/// Hace que el personaje (o sprite) siempre mire hacia la cámara principal (efecto billboard estilo Paper Mario).
/// </summary>
public class BillboardCharacter : MonoBehaviour
{
    private Transform camTransform;

    void Awake()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (camTransform != null)
        {
            Vector3 camForward = camTransform.forward;
            camForward.y = 0; // Opcional: solo rota en el eje Y
            transform.forward = camForward;
        }
    }
}
