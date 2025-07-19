using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Configuración de inversión")]
        [SerializeField] private float tiempoParaInvertirCamara = 1.0f;
        private float tiempoAcercandoseALaCamara = 0.0f;

        [Header("Cámaras hijas")]
        [SerializeField] private string nombreCamaraTrasera = "Camera";
        [SerializeField] private string nombreCamaraFrontal = "Camera1";
        private Camera camaraTrasera, camaraFrontal;
        private Camera activeCamera;

        /// <summary>
        /// Evento que se dispara cuando la cámara activa cambia.
        /// </summary>
        public event System.Action<Camera> OnCameraChanged;

        void Awake()
        {
            // Inicializa referencias a las cámaras hijas automáticamente
            camaraTrasera = transform.Find(nombreCamaraTrasera)?.GetComponent<Camera>();
            camaraFrontal = transform.Find(nombreCamaraFrontal)?.GetComponent<Camera>();
            // Por defecto activa la trasera si existe
            if (camaraTrasera != null)
                SetActiveCamera(camaraTrasera);
        }

        /// <summary>
        /// Asigna la cámara activa y dispara el evento de cambio.
        /// </summary>
        public void SetActiveCamera(Camera cam)
        {
            if (cam == null)
            {
                Debug.LogError("PlayerCameraController: Cámara asignada es nula.");
                return;
            }
            if (activeCamera != null)
                activeCamera.gameObject.SetActive(false);
            activeCamera = cam;
            activeCamera.gameObject.SetActive(true);
            OnCameraChanged?.Invoke(activeCamera);
        }

        /// <summary>
        /// Invierte entre la cámara trasera y frontal.
        /// </summary>
        public void InvertirCamara()
        {
            if (camaraTrasera == null || camaraFrontal == null)
            {
                Debug.LogError("PlayerCameraController: No se encontraron las cámaras hijas correctamente.");
                return;
            }
            SetActiveCamera(activeCamera == camaraTrasera ? camaraFrontal : camaraTrasera);
        }

        /// <summary>
        /// Detecta si el jugador está presionando el input de movimiento hacia abajo y la invierte si corresponde.
        /// Debe llamarse desde el controlador de movimiento, pasando IsMovingDown.
        /// </summary>
        public void DetectarAcercamiento(bool isMovingDown)
        {
            if (activeCamera == null) return;
            if (isMovingDown)
            {
                tiempoAcercandoseALaCamara += Time.deltaTime;
                if (tiempoAcercandoseALaCamara >= tiempoParaInvertirCamara)
                {
                    InvertirCamara();
                    tiempoAcercandoseALaCamara = 0.0f;
                }
            }
            else
            {
                tiempoAcercandoseALaCamara = 0.0f;
            }
        }

        /// <summary>
        /// Permite obtener la cámara activa actual.
        /// </summary>
        public Camera GetActiveCamera() => activeCamera;
    }
}