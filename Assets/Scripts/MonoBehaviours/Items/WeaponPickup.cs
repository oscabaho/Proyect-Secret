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
    [Header("Sonido de recogida")]
    [SerializeField] private AudioClip PickupSound; // Sonido que se reproduce al recoger el arma

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
        var equipmentController = other.GetComponent<PlayerEquipmentController>();

        // Usamos "guard clauses" para salir temprano si algo falta. Es más limpio.
        if (equipmentController == null || weaponItem == null)
        {
            #if UNITY_EDITOR
            if (equipmentController == null) Debug.LogWarning($"WeaponPickup: El objeto '{other.name}' no tiene PlayerEquipmentController.");
            if (weaponItem == null) Debug.LogWarning($"WeaponPickup: No hay un 'Weapon Item' asignado en el Inspector de '{gameObject.name}'.");
            #endif
            return;
        }

        equipmentController.EquipWeapon(weaponItem);
        SoundManager.Instancia.ReproducirEfecto(PickupSound);
        if (interactionTrigger != null)
            interactionTrigger.SetActive(false);
        Destroy(gameObject);
    }
}
