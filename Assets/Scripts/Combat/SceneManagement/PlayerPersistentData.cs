using UnityEngine;

namespace Combat.SceneManagement
{
    /// <summary>
    /// ScriptableObject para transferir datos persistentes del jugador entre escenas.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Combat/PlayerPersistentData")]
    public class PlayerPersistentData : ScriptableObject
    {
        // Ejemplo: inventario, experiencia, salud, etc.
        public int playerHealth;
        public int playerExperience;
        // Puedes agregar más campos según lo que quieras persistir

        public void SaveFromPlayer(GameObject player)
        {
            // Aquí puedes extraer datos del jugador y guardarlos
            // Ejemplo: playerHealth = player.GetComponent<HealthComponent>().CurrentValue;
        }

        public void ApplyToPlayer(GameObject player)
        {
            // Aquí puedes aplicar los datos guardados al jugador al regresar
            // Ejemplo: player.GetComponent<HealthComponent>().SetValue(playerHealth);
        }
    }
}
