using UnityEngine;
using ProyectSecret.Inventory.Items;
using ProyectSecret.Inventory;

/// <summary>
/// Permite al jugador recoger un arma del suelo y equiparla automáticamente.
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeaponPickup : MonoBehaviour
{
    [Header("ScriptableObject del arma")]
    [SerializeField] private WeaponItem weaponItem; // Asigna el ScriptableObject en el inspector
    [Header("Trigger de interacción (hijo)")]
    [SerializeField] private GameObject interactionTrigger; // Asigna el hijo con el collider trigger

    private void Awake()
    {
        if (interactionTrigger == null)
        {
            // Busca el primer hijo con collider trigger
            foreach (Transform child in transform)
            {
                var col = child.GetComponent<Collider>();
                if (col != null && col.isTrigger)
                {
                    interactionTrigger = child.gameObject;
                    //Debug.Log($"WeaponPickup: Trigger hijo encontrado: {interactionTrigger.name}");
                    break;
                }
            }
        }
        //Debug.Log($"WeaponPickup: Awake en {gameObject.name}, weaponItem asignado: {(weaponItem != null ? weaponItem.name : "null")}, trigger: {(interactionTrigger != null ? interactionTrigger.name : "null")}");
    }

    // Este método es llamado por el hijo trigger (WeaponPickupTrigger)
    public void OnPickupTriggered(Collider other)
    {
        //Debug.Log($"WeaponPickup: Trigger activado por {other.name}");
        var equipmentController = other.GetComponent<PlayerEquipmentController>();
        if (equipmentController == null)
        {
            Debug.LogWarning($"WeaponPickup: {other.name} no tiene PlayerEquipmentController");
        }
        if (weaponItem == null)
        {
            Debug.LogWarning($"WeaponPickup: weaponItem no asignado en {gameObject.name}");
        }
        if (equipmentController != null && weaponItem != null)
        {
            //Debug.Log($"WeaponPickup: Equipando arma {weaponItem.name} en {other.name}");
            equipmentController.EquipWeapon(weaponItem); // Equipa la daga al jugador
            if (interactionTrigger != null)
            {
                interactionTrigger.SetActive(false); // Desactiva el trigger de interacción
                //Debug.Log($"WeaponPickup: Trigger {interactionTrigger.name} desactivado");
            }
            //Debug.Log($"WeaponPickup: Destruyendo {gameObject.name} tras recogida");
            Destroy(gameObject); // Elimina la daga del suelo
        }
    }
}
