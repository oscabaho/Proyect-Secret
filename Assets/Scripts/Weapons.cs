using Base;
using UnityEngine;

public abstract class Weapons : ScriptableObject
{
    [SerializeField]private string weaponName;
    [SerializeField]private string weaponType;
    [SerializeField]private int damage;
    
    public virtual void Attack(RaycastHit hit)
    {
        var damageable = hit.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"ataque a {hit.transform.name} por {damage} de daño.");
        }
        else Debug.Log("Attack executed");
    }
}
