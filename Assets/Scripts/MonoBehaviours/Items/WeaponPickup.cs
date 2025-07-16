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
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var equipmentController = other.GetComponent<PlayerEquipmentController>();
        if (equipmentController != null && weaponItem != null)
        {
            equipmentController.EquipWeapon(weaponItem); // Equipa la daga al jugador
            if (interactionTrigger != null)
                interactionTrigger.SetActive(false); // Desactiva el trigger de interacción
            Destroy(gameObject); // Elimina la daga del suelo
        }
    }
}
