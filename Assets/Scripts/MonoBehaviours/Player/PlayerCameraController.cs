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

        private PaperMarioPlayerMovement playerMovement;

        /// <summary>
        /// Evento que se dispara cuando la cámara activa cambia.
        /// </summary>
        public event System.Action<Camera> OnCameraChanged;

        void Awake()
        {
            playerMovement = GetComponent<PaperMarioPlayerMovement>();

            // Inicializa referencias a las cámaras hijas automáticamente
            camaraTrasera = transform.Find(nombreCamaraTrasera)?.GetComponent<Camera>();
            camaraFrontal = transform.Find(nombreCamaraFrontal)?.GetComponent<Camera>();
            
            // Por defecto activa la trasera si existe
            if (camaraTrasera != null)
                SetActiveCamera(camaraTrasera);
        }

        private void Update()
        {
            if (playerMovement == null || activeCamera == null) return;

            // La lógica de 'DetectarAcercamiento' ahora vive aquí, haciendo este componente autocontenido.
            if (playerMovement.IsMovingDown)
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
            
            // Notificar al sistema que el estado de inversión ha cambiado.
            // Esto es crucial para que otros componentes (como el de equipamiento) reaccionen.
            if (playerMovement != null)
            {
                playerMovement.SetCameraInverted(activeCamera == camaraFrontal);
            }
        }

        /// <summary>
        /// Permite obtener la cámara activa actual.
        /// </summary>
        public Camera GetActiveCamera() => activeCamera;
    }
}
