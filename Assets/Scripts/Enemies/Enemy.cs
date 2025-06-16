using System;
using UnityEngine;
using Enemies;

/// <summary>
/// Controlador principal del enemigo. Orquesta IA, ataques y referencia a Enemies.EnemyHealthController.
/// </summary>
[RequireComponent(typeof(Enemies.EnemyHealthController))]
public class Enemy : MonoBehaviour
{
    private Enemies.EnemyHealthController healthController;

    private void Awake()
    {
        healthController = GetComponent<Enemies.EnemyHealthController>();
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
