using System;
using UnityEngine;
using Interfaces;
using System.Linq;

/// <summary>
/// Ejemplo de suscriptor a eventos de muerte para logros.
/// </summary>
public class AchievementSystem : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus.Instance.Subscribe<CharacterDeathEvent>(OnCharacterDeath);
    }

    private void OnDisable()
    {
        GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(OnCharacterDeath);
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
