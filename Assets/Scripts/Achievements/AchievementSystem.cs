using System;
using UnityEngine;
using ProyectSecret.Interfaces;
using System.Linq;
using ProyectSecret.Events;


namespace ProyectSecret.Achievements
{
    /// <summary>
    /// Ejemplo de suscriptor a eventos de muerte para logros.
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        private void OnEnable()
        {
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<CharacterDeathEvent>(OnCharacterDeath);
        }

        private void OnDisable()
        {
            ProyectSecret.Events.GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(OnCharacterDeath);
        }

        private void OnCharacterDeath(CharacterDeathEvent evt)
        {
            // Ejemplo: desbloquear logro si el enemigo derrotado es de cierto tipo
            if (evt.Character.CompareTag("Enemy"))
            {
                Debug.Log("¡Logro desbloqueado: Derrotaste a un enemigo!");
                // Aquí puedes marcar el logro como desbloqueado
            }
        }
    }
}
