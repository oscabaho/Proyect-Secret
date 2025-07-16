using UnityEngine;
using ProyectSecret.Inventory.Items;
using ProyectSecret.Inventory;

/// <summary>
/// Script para el hijo trigger del arma. Detecta la interacción y notifica al padre para recoger/equipar el arma.
/// </summary>
public class WeaponPickupTrigger : MonoBehaviour
{
    private WeaponPickup parentPickup;

    private void Awake()
    {
        parentPickup = GetComponentInParent<WeaponPickup>();
        if (parentPickup == null)
            Debug.LogWarning($"WeaponPickupTrigger: No se encontró WeaponPickup en el padre de {gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"WeaponPickupTrigger: Trigger activado por {other.name}");
        if (parentPickup != null)
            parentPickup.OnPickupTriggered(other);
    }
}
