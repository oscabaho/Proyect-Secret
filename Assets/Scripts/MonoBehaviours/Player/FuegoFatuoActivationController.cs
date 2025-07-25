using UnityEngine;
using ProyectSecret.Events;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Responsabilidad Única: Activa o desactiva el GameObject del Fuego Fatuo
    /// en respuesta a los eventos de día y noche.
    /// </summary>
    public class FuegoFatuoActivationController : MonoBehaviour
    {
        [Header("Referencias")]
        [Tooltip("Arrastra aquí el GameObject hijo que contiene el Fuego Fatuo.")]
        [SerializeField] private GameObject fuegoFatuoObject;

        private void Awake()
        {
            if (fuegoFatuoObject == null)
            {
                Debug.LogError("FuegoFatuoActivationController: No se ha asignado el GameObject del Fuego Fatuo en el Inspector.", this);
                enabled = false;
                return;
            }
            // Asegurarse de que empieza desactivado.
            fuegoFatuoObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEventBus.Instance?.Subscribe<DayStartedEvent>(evt => fuegoFatuoObject.SetActive(false));
            GameEventBus.Instance?.Subscribe<NightStartedEvent>(evt => fuegoFatuoObject.SetActive(true));
        }

        private void OnDisable()
        {
            GameEventBus.Instance?.Unsubscribe<DayStartedEvent>(evt => fuegoFatuoObject.SetActive(false));
            GameEventBus.Instance?.Unsubscribe<NightStartedEvent>(evt => fuegoFatuoObject.SetActive(true));
        }
    }
}