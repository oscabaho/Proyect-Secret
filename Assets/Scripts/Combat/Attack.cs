using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using Base;

/// <summary>
/// Permite atacar a cualquier objeto que implemente IDamageable.
/// </summary>
public class Attack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damage = 10f;
    private InputAction attack;
    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        attack = InputSystem.actions.FindAction("Attack");
        if (playerCamera == null)
            Debug.LogWarning("Attack: No se encontró cámara en el jugador.");
    }

    void Update()
    {
        if (attack.WasPressedThisFrame())
        {
            Attacking();
        }
        
    }

    void Attacking()
    {
        RaycastHit hit;

        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, attackRange))
        {
            Debug.Log(hit.transform.name);

            var damageable = hit.transform.GetComponent<Base.IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage((int)damage);
            }
        }
    }
}
