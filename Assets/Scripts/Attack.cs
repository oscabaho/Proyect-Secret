using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    private InputAction attack;
    private Camera playerCamera;
    [SerializeField]private float attackRange = 2f;
    [SerializeField]private float damage = 10f;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        attack = InputSystem.actions.FindAction("Attack");
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

            Enemy enemy = hit.transform.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.RecieveDamage(damage);
            }
        }
    }
}
