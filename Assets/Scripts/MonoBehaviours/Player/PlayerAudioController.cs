using UnityEngine;
using ProyectSecret.Audio;
using ProyectSecret.Managers;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Responsabilidad Única: Gestionar los efectos de sonido del jugador.
    /// </summary>
    [RequireComponent(typeof(PaperMarioPlayerMovement), typeof(PlayerInputController))]
    public class PlayerAudioController : MonoBehaviour
    {
        [Header("Audio Data")]
        [SerializeField] private AudioData moveSoundData;
        [SerializeField] private AudioData jumpSoundData;

        private PaperMarioPlayerMovement _movement;
        private PlayerInputController _input;
        private bool _wasMovingLastFrame = false;

        private void Awake()
        {
            _movement = GetComponent<PaperMarioPlayerMovement>();
            _input = GetComponent<PlayerInputController>();
        }

        private void OnEnable()
        {
            if (_input != null) _input.OnJumpPressed += PlayJumpSound;
        }

        private void OnDisable()
        {
            if (_input != null) _input.OnJumpPressed -= PlayJumpSound;
        }

        private void Update()
        {
            bool isCurrentlyMoving = _movement.CurrentVelocity.sqrMagnitude > 0.01f;

            if (isCurrentlyMoving && !_wasMovingLastFrame)
                AudioManager.Instance?.PlayLoopingSoundOnObject(moveSoundData, gameObject);
            else if (!isCurrentlyMoving && _wasMovingLastFrame)
                AudioManager.Instance?.StopLoopingSoundOnObject(gameObject);

            _wasMovingLastFrame = isCurrentlyMoving;
        }

        private void PlayJumpSound()
        {
            if (_movement.IsGrounded) jumpSoundData?.Play();
        }
    }
}