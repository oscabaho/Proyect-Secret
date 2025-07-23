using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Responsabilidad Única: Controlar el Animator y el SpriteRenderer del jugador.
    /// Lee el estado de otros componentes (Input, Physics) para actualizar la vista.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerInputController), typeof(PaperMarioPlayerMovement))]
    public class PlayerAnimationController : MonoBehaviour
    {
        // Referencias a componentes
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private PlayerInputController _input;
        private PaperMarioPlayerMovement _movement;

        // Hashes de parámetros del Animator para optimización
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _isMovingHash = Animator.StringToHash("IsMoving");
        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int _jumpTriggerHash = Animator.StringToHash("Jump");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _input = GetComponent<PlayerInputController>();
            _movement = GetComponent<PaperMarioPlayerMovement>();
        }

        private void OnEnable()
        {
            if (_input != null) _input.OnJumpPressed += TriggerJumpAnimation;
        }

        private void OnDisable()
        {
            if (_input != null) _input.OnJumpPressed -= TriggerJumpAnimation;
        }

        private void Update()
        {
            Vector2 moveInput = _input.MoveInput;

            _animator.SetFloat(_moveXHash, moveInput.x);
            _animator.SetFloat(_moveYHash, moveInput.y);
            _animator.SetBool(_isMovingHash, moveInput.sqrMagnitude > 0.01f);
            _animator.SetBool(_isGroundedHash, _movement.IsGrounded);

            // FlipX para mirar a la derecha/izquierda según el input
            if (Mathf.Abs(moveInput.x) > 0.1f)
            {
                _spriteRenderer.flipX = moveInput.x < 0f;
            }
        }

        private void TriggerJumpAnimation()
        {
            if (_movement.IsGrounded) _animator.SetTrigger(_jumpTriggerHash);
        }
    }
}