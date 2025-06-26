using UnityEngine;

[CreateAssetMenu(fileName = "New Axe", menuName = "Weapons/Axe")]
public class Axe : Weapons
{
    public override void Attack(RaycastHit hit)
    {
        base.Attack(hit);

        Debug.Log("Sword attack executed!");
    }
}
