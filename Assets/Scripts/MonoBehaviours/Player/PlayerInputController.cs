using UnityEngine;
using UnityEngine.InputSystem;
using System;
using ProyectSecret.Events;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Responsabilidad Única: Gestionar todas las entradas del jugador desde InputActionAsset.
    /// Expone los valores de entrada y eventos para que otros componentes los consuman.
    /// </summary>
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Input Asset")]
        [field: SerializeField]
        public InputActionAsset InputActions { get; private set; }

        [Header("Action Map Names")]
        [SerializeField] private string dayActionMapName = "PlayerDay";
        [SerializeField] private string nightActionMapName = "PlayerNight";

        [Header("Action Names")]
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string attackActionName = "Attack";
        [SerializeField] private string interactActionName = "Interact";

        // Eventos públicos para acciones
        public event Action OnAttackPressed;
        public event Action OnInteractPressed;

        // Propiedades públicas para estados
        public Vector2 MoveInput { get; private set; }

        private InputActionMap _currentActionMap;
        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _interactAction;

        private void Awake()
        {
            // Inicializa con el mapa de día por defecto
            SwitchActionMap(dayActionMapName);
        }

        private void OnEnable()
        {
            GameEventBus.Instance?.Subscribe<DayStartedEvent>(OnDayStarted);
            GameEventBus.Instance?.Subscribe<NightStartedEvent>(OnNightStarted);
            // Las suscripciones a las acciones ahora se gestionan en SwitchActionMap
        }

        private void OnDisable()
        {
            GameEventBus.Instance?.Unsubscribe<DayStartedEvent>(OnDayStarted);
            GameEventBus.Instance?.Unsubscribe<NightStartedEvent>(OnNightStarted);
            
            // Desactivamos el mapa actual para limpiar todo
            _currentActionMap?.Disable();
        }

        private void Update()
        {
            MoveInput = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        private void HandleAttack(InputAction.CallbackContext context) => OnAttackPressed?.Invoke();
        private void HandleInteract(InputAction.CallbackContext context) => OnInteractPressed?.Invoke();
        private void OnDayStarted(DayStartedEvent evt) => SwitchActionMap(dayActionMapName);
        private void OnNightStarted(NightStartedEvent evt) => SwitchActionMap(nightActionMapName);

        private void SwitchActionMap(string mapName)
        {
            // 1. Desuscribirse de las acciones del mapa anterior para evitar listeners duplicados o huérfanos.
            if (_attackAction != null) _attackAction.performed -= HandleAttack;
            if (_interactAction != null) _interactAction.performed -= HandleInteract;

            // 2. Desactivar el mapa de acción actual.
            _currentActionMap?.Disable();

            // 3. Encontrar y asignar el nuevo mapa y sus acciones.
            _currentActionMap = InputActions.FindActionMap(mapName);
            _moveAction = _currentActionMap.FindAction(moveActionName);
            _attackAction = _currentActionMap.FindAction(attackActionName);
            _interactAction = _currentActionMap.FindAction(interactActionName);

            // 4. Suscribirse a los eventos de las NUEVAS acciones.
            if (_attackAction != null)
            {
                _attackAction.performed += HandleAttack;
            }
            if (_interactAction != null)
            {
                _interactAction.performed += HandleInteract;
            }

            // 5. Activar el nuevo mapa.
            _currentActionMap?.Enable();
        }
    }
}