using UnityEngine;
using System;

/// <summary>
/// Ejemplo de sistema de misiones que reacciona a eventos globales del Event Bus.
/// </summary>
public class QuestSystem : MonoBehaviour
{
    private int enemiesDefeated = 0;
    private int itemsUsed = 0;
    private bool combatWon = false;

    private void OnEnable()
    {
        GameEventBus.Instance.Subscribe<CharacterDeathEvent>(OnCharacterDeath);
        GameEventBus.Instance.Subscribe<CombatVictoryEvent>(OnCombatVictory);
        GameEventBus.Instance.Subscribe<ItemUsedEvent>(OnItemUsed);
    }

    private void OnDisable()
    {
        GameEventBus.Instance.Unsubscribe<CharacterDeathEvent>(OnCharacterDeath);
        GameEventBus.Instance.Unsubscribe<CombatVictoryEvent>(OnCombatVictory);
        GameEventBus.Instance.Unsubscribe<ItemUsedEvent>(OnItemUsed);
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
        combatWon = true;
        Debug.Log("Misión: ¡Victoria en combate!");
        // Aquí puedes marcar la misión como completada
    }

    private void OnItemUsed(ItemUsedEvent evt)
    {
        itemsUsed++;
        Debug.Log($"Misión: Ítems usados = {itemsUsed}");
        // Aquí puedes comprobar si se cumple una misión de uso de ítems
    }
}
