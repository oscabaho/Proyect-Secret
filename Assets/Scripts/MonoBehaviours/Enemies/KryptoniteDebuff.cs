using System;
using UnityEngine;
using ProyectSecret.Interfaces;
// using Enemies; // Eliminar si no es necesario
using ProyectSecret.Characters.Enemies;

namespace MonoBehaviours.Enemies
{
    /// <summary>
    /// Componente de kriptonita: debilita al enemigo si el jugador tiene el objeto indicado.
    /// Aplica un multiplicador al daño recibido.
    /// </summary>
    public class KryptoniteDebuff : MonoBehaviour
    {
        [SerializeField] private string kryptoniteItemId = "cebolla";
        [SerializeField] private float damageMultiplier = 2f; // El daño recibido se multiplica por este valor
        private bool isDebuffed = false;

        // Referencia al controlador de salud modular
        [SerializeField] private EnemyHealthController healthController;

        private void Awake()
        {
            if (healthController == null)
                healthController = GetComponent<EnemyHealthController>();
            if (healthController != null)
                healthController.OnPreTakeDamage += OnPreTakeDamageHandler;
        }

        public void CheckKryptonite(GameObject player)
        {
            var inventory = player.GetComponent<IInventory>();
            if (inventory != null && inventory.HasItem(kryptoniteItemId))
            {
                ApplyDebuff();
            }
            else
            {
                RemoveDebuff();
            }
        }

        private void ApplyDebuff()
        {
            if (isDebuffed) return;
            isDebuffed = true;
            Debug.Log($"{gameObject.name} recibirá daño multiplicado por {damageMultiplier} debido a la kriptonita: {kryptoniteItemId}");
        }

        private void RemoveDebuff()
        {
            if (!isDebuffed) return;
            isDebuffed = false;
            Debug.Log($"{gameObject.name} restaurado a daño normal");
        }

        private int OnPreTakeDamageHandler(int baseDamage)
        {
            if (isDebuffed)
                return Mathf.RoundToInt(baseDamage * damageMultiplier);
            return baseDamage;
        }

        private void OnDestroy()
        {
            if (healthController != null)
                healthController.OnPreTakeDamage -= OnPreTakeDamageHandler;
        }
    }
}
