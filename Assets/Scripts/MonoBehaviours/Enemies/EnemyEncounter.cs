using UnityEngine;
using ProyectSecret.Managers;

public class EnemyEncounter : MonoBehaviour // Este script estaría en el enemigo en la escena de exploración
{
    [SerializeField] private GameObject enemyCombatPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<ProyectSecret.Characters.Player.PlayerController>();
            if (playerController != null)
            {
                GameObject playerPrefab = other.gameObject; // ¡OJO! Esto no es el prefab, es la instancia.
                                                            // Lo ideal es tener una referencia al prefab original.

                SceneTransitionManager.Instance?.LoadCombatScene(playerPrefab, enemyCombatPrefab, other.gameObject);
                
                gameObject.SetActive(false);
            }
        }
    }
}
