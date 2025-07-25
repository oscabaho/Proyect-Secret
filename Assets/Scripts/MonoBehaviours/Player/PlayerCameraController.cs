using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    [RequireComponent(typeof(PaperMarioPlayerMovement), typeof(PlayerInputController))]
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Configuración de inversión")]
        [SerializeField] private float tiempoParaInvertirCamara = 1.0f;
        private float tiempoAcercandoseALaCamara = 0.0f;

        [Header("Cámaras hijas")]
        [Tooltip("Arrastra aquí el GameObject de la cámara trasera.")]
        [SerializeField] private Camera camaraTrasera;
        [Tooltip("Arrastra aquí el GameObject de la cámara frontal.")]
        [SerializeField] private Camera camaraFrontal;

        [Header("Configuración de Rotación (Día)")]
        [Tooltip("El objeto vacío que actúa como pivote para la rotación orbital de la cámara.")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float lookSensitivity = 100f;
        [Tooltip("Velocidad con la que la cámara vuelve a su posición inicial al llegar la noche.")]
        [SerializeField] private float resetSpeed = 15f;
        
        private Camera activeCamera;
        private PaperMarioPlayerMovement playerMovement;
        private Quaternion _initialPivotRotation;
        private PlayerInputController _input;

        /// <summary>
        /// Evento que se dispara cuando la cámara activa cambia.
        /// </summary>
        public event System.Action<Camera> OnCameraChanged;

        void Awake()
        {
            playerMovement = GetComponent<PaperMarioPlayerMovement>();
            _input = GetComponent<PlayerInputController>();

            // Validar que las cámaras han sido asignadas en el Inspector.
            if (camaraTrasera == null || camaraFrontal == null || cameraPivot == null)
            {
                #if UNITY_EDITOR
                Debug.LogError("PlayerCameraController: Faltan referencias a las cámaras o al pivote. Por favor, asígnalas en el Inspector.");
                #endif
                enabled = false; // Desactivamos el componente para evitar más errores.
                return;
            }

            // Guardamos la rotación inicial del pivote para poder resetearla.
            _initialPivotRotation = cameraPivot.rotation;
            
            // Por defecto activa la trasera.
            SetActiveCamera(camaraTrasera);
        }

        private void Update()
        {
            if (playerMovement == null || activeCamera == null || _input == null) return;

            // Lógica de rotación de cámara durante el día
            if (_input.CurrentActionMapName == "PlayerDay")
            {
                float lookX = _input.LookInput.x * lookSensitivity * Time.deltaTime;
                // Rotamos el pivote en el eje Y (vertical) para conseguir el movimiento circular horizontal.
                cameraPivot.Rotate(Vector3.up, lookX);
            }
            else
            {
                // Si no es de día, hacemos una transición suave de la rotación del pivote a su estado original.
                // Slerp (Spherical Linear Interpolation) es ideal para rotaciones.
                cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, _initialPivotRotation, Time.deltaTime * resetSpeed);
            }

            // Lógica de inversión de cámara al acercarse (funciona en ambos modos)
            // Nota: Si no quieres que esto pase durante el día, puedes añadir un `if (_input.CurrentActionMapName == "PlayerNight")`
            // alrededor de este bloque.
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
                Debug.LogError("PlayerCameraController: Se intentó activar una cámara nula.");
                return;
            }

            // Desactivar todas las cámaras para asegurar que solo una esté activa.
            camaraTrasera.gameObject.SetActive(false);
            camaraFrontal.gameObject.SetActive(false);

            activeCamera = cam;
            activeCamera.gameObject.SetActive(true);
            OnCameraChanged?.Invoke(activeCamera);
        }

        /// <summary>
        /// Invierte entre la cámara trasera y frontal.
        /// </summary>
        public void InvertirCamara()
        {
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

        /// <summary>
        /// Comprueba si el pivote de la cámara es hijo de un transform específico.
        /// Útil para evitar bucles de retroalimentación con el billboard del sprite.
        /// </summary>
        public bool IsCameraPivotChildOf(Transform potentialParent)
        {
            return cameraPivot != null && cameraPivot.IsChildOf(potentialParent);
        }
    }
}
