using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// --- a/e:\osbah\Entregables\Proyect-Secret\Assets\Scripts\MonoBehaviours\Player\PlayerAnimationController.cs
    /// +++ b/e:\osbah\Entregables\Proyect-Secret\Assets\Scripts\MonoBehaviours\Player\PlayerAnimationController.cs
    /// Responsabilidad Única: Controlar el Animator y el SpriteRenderer del jugador.
    /// Lee el estado de otros componentes (Input, Physics) para actualizar la vista.
    /// </summary>
    [RequireComponent(typeof(PlayerInputController))]
    [RequireComponent(typeof(PaperMarioPlayerMovement), typeof(PlayerCameraController))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Componentes Visuales (Hijos)")]
        [Tooltip("Referencia al Animator que está en el objeto hijo de los visuales.")]
        [SerializeField] private Animator _animator;
        [Tooltip("Referencia al SpriteRenderer que está en el objeto hijo de los visuales.")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        // Referencias a componentes del objeto raíz
        private PlayerInputController _input;
        private PaperMarioPlayerMovement _movement;
        private PlayerCameraController _cameraController;

        // Hashes de parámetros del Animator para optimización
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _isMovingHash = Animator.StringToHash("IsMoving");
        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");

        private void Awake()
        {
            _input = GetComponent<PlayerInputController>();
            _movement = GetComponent<PaperMarioPlayerMovement>();
            _cameraController = GetComponent<PlayerCameraController>();

            if (_animator == null || _spriteRenderer == null)
            {
                Debug.LogError("PlayerAnimationController: El Animator o el SpriteRenderer no están asignados en el Inspector. Por favor, arrastra el objeto hijo que los contiene a las ranuras correspondientes.", this);
                enabled = false;
                return;
            }

#if UNITY_EDITOR
            ValidateAnimatorParameters();

            // Salvaguarda para evitar bucles de retroalimentación.
            if (_cameraController.IsCameraPivotChildOf(_spriteRenderer.transform))
            {
                Debug.LogError(
                    "PlayerAnimationController: ¡Configuración de jerarquía peligrosa detectada! " +
                    "El 'CameraPivot' no puede ser hijo del objeto que contiene el SpriteRenderer, ya que esto crea un bucle de rotación. " +
                    "El efecto billboard será desactivado.", this);
            }
#endif
        }

        private void Update()
        {
            // Durante el día, el sprite debe mirar a la cámara (efecto billboard).
            // Solo aplicamos el billboard si la jerarquía es segura.
            if (_input.CurrentActionMapName == "PlayerDay" && !_cameraController.IsCameraPivotChildOf(_spriteRenderer.transform))
            {
                Camera activeCam = _cameraController.GetActiveCamera();
                if (activeCam != null)
                {
                    // Hacemos que el sprite copie la rotación de la cámara.
                    _spriteRenderer.transform.rotation = activeCam.transform.rotation;
                }
            }
            else // Durante la noche, el comportamiento es el normal.
            {
                // ¡IMPORTANTE! Reseteamos la rotación del sprite a su estado por defecto.
                // Sin esto, el sprite se queda "atascado" con la última rotación del modo día,
                // rompiendo el efecto billboard y haciendo que el movimiento parezca incorrecto.
                _spriteRenderer.transform.localRotation = Quaternion.identity;

                Vector2 moveInput = _input.MoveInput;

                _animator.SetFloat(_moveXHash, moveInput.x);
                _animator.SetFloat(_moveYHash, moveInput.y);
                _animator.SetBool(_isMovingHash, moveInput.sqrMagnitude > 0.01f);

                // FlipX para mirar a la derecha/izquierda según el input
                if (Mathf.Abs(moveInput.x) > 0.01f) // Usar un umbral más pequeño para mayor respuesta
                {
                    // La lógica de flip debe tener en cuenta la inversión de la cámara.
                    bool shouldFlip = _movement.IsCameraInverted ? moveInput.x > 0f : moveInput.x < 0f;
                    _spriteRenderer.flipX = shouldFlip;
                }
            }

            // La animación de IsGrounded es independiente del estado día/noche.
            _animator.SetBool(_isGroundedHash, _movement.IsGrounded);
        }

#if UNITY_EDITOR
        // Este método se ejecuta solo en el editor para advertir sobre parámetros faltantes.
        private void ValidateAnimatorParameters()
        {
            if (_animator == null || _animator.runtimeAnimatorController == null) return;

            var parameters = new System.Collections.Generic.HashSet<string>();
            foreach (var param in _animator.parameters)
            {
                parameters.Add(param.name);
            }

            if (!parameters.Contains("MoveX")) Debug.LogWarning("PlayerAnimationController: Falta el parámetro 'MoveX' (Float) en el Animator.", this);
            if (!parameters.Contains("MoveY")) Debug.LogWarning("PlayerAnimationController: Falta el parámetro 'MoveY' (Float) en el Animator.", this);
            if (!parameters.Contains("IsMoving")) Debug.LogWarning("PlayerAnimationController: Falta el parámetro 'IsMoving' (Bool) en el Animator.", this);
            if (!parameters.Contains("IsGrounded")) Debug.LogWarning("PlayerAnimationController: Falta el parámetro 'IsGrounded' (Bool) en el Animator.", this);
        }
#endif
    }
}