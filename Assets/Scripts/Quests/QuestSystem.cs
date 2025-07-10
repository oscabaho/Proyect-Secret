using UnityEngine;
using System;
using ProyectSecret.Events;

namespace ProyectSecret.Quests
{
    /// <summary>
    /// Ejemplo de sistema de misiones que reacciona a eventos globales del Event Bus.
    /// </summary>
    public class QuestSystem : MonoBehaviour
    {
        private int enemiesDefeated = 0;
        private int itemsUsed = 0;

        private void OnEnable()
        {
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<CharacterDeathEvent>(OnCharacterDeath);
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<CombatVictoryEvent>(OnCombatVictory);
            ProyectSecret.Events.GameEventBus.Instance.Subscribe<ItemUsedEvent>(OnItemUsed);
        }

        private void OnDisable()
        {
            ProyectSecret.Events.GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(OnCharacterDeath);
            ProyectSecret.Events.GameEventBus.Instance.Unsubscribe<CombatVictoryEvent>(OnCombatVictory);
            ProyectSecret.Events.GameEventBus.Instance.Unsubscribe<ItemUsedEvent>(OnItemUsed);
        }

        private void OnCharacterDeath(CharacterDeathEvent evt)
        {
            if (evt.Character.CompareTag("Enemy"))
            {
                enemiesDefeated++;
                Debug.Log($"Misión: Enemigos derrotados = {enemiesDefeated}");
                // Aquí puedes comprobar si se cumple una misión
            }
        }

        private void OnCombatVictory(CombatVictoryEvent evt)
        {
            Debug.Log("Misión: ¡Victoria en combate!");
        }

        private void OnItemUsed(ItemUsedEvent evt)
        {
            itemsUsed++;
            Debug.Log($"Misión: Ítems usados = {itemsUsed}");
            // Aquí puedes comprobar si se cumple una misión de uso de ítems
        }
    }
}
