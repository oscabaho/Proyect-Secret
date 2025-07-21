using UnityEngine;
using ProyectSecret.Managers;

public class EnemyEncounter : MonoBehaviour // Este script estaría en el enemigo en la escena de exploración
{
    [SerializeField] private GameObject enemyCombatPrefab;

    private void OnTriggerEnter(Collider other)
    {
        // Usar CompareTag es más eficiente y desacopla la lógica del PlayerController.
        if (other.CompareTag("Player")) 
        {
            // NOTA: La obtención del 'playerPrefab' debe mejorarse. Lo ideal es tener una referencia
            // al prefab original, quizás a través de un ScriptableObject o un manager.
            GameObject playerPrefab = other.gameObject; // ¡OJO! Esto no es el prefab, es la instancia.

            SceneTransitionManager.Instance?.LoadCombatScene(playerPrefab, enemyCombatPrefab, other.gameObject);
            gameObject.SetActive(false);
        }
    }
}
