using System;
using UnityEngine;
using ProyectSecret.Characters.Enemies;

namespace MonoBehaviours.Enemies
{
    /// <summary>
    /// Controlador principal del enemigo. Orquesta IA, ataques y referencia a EnemyHealthController.
    /// </summary>
    [RequireComponent(typeof(EnemyHealthController))]
    public class Enemy : MonoBehaviour
    {
    private EnemyHealthController healthController;


    private void Awake()
    {
        healthController = GetComponent<EnemyHealthController>();
        if (healthController == null)
            Debug.LogError("Enemy: No se encontró EnemyHealthController.");
    }

    public void TakeDamage(int amount)
    {
        healthController?.TakeDamage(amount);
    }

    public void SubscribeOnDeath(Action callback)
    {
        if (healthController != null)
            healthController.OnDeath += callback;
    }
    public void UnsubscribeOnDeath(Action callback)
    {
        if (healthController != null)
            healthController.OnDeath -= callback;
    }

    // Puedes agregar aquí la lógica de IA, ataques, percepción, etc.
    }
}