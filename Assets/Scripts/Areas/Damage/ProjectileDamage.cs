using UnityEngine;
using Components;
using Base;

/// <summary>
/// ProjectileDamage
/// Es un componente que se adjunta al prefab del proyectil.
/// Su función es aplicar daño cuando el proyectil colisiona con un objeto que implemente HealthComponent.
/// Se encarga de la lógica de colisión y destrucción del proyectil tras impactar.
/// </summary>
public class ProjectileDamage : MonoBehaviour
{
    private int damage = 0;
    private GameObject owner;

    public void SetDamage(int dmg)
    {
        damage = Mathf.Max(0, dmg);
    }

    public void SetOwner(GameObject ownerObj)
    {
        owner = ownerObj;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Evita dañar al lanzador
        if (owner != null && other.gameObject == owner)
            return;

        var statHolder = other.GetComponent<BaseStatHolder>();
        var health = statHolder != null ? statHolder.Health : null;
        if (health != null && damage > 0)
        {
            health.AffectValue(-damage);
            Destroy(gameObject);
        }
    }
}
