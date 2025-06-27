using UnityEngine;

/// <summary>
/// Hace que el personaje (o sprite) siempre mire hacia la cámara principal (efecto billboard estilo Paper Mario).
/// </summary>
public class BillboardCharacter : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0; // Opcional: solo rota en el eje Y
            transform.forward = camForward;
        }
    }
}
