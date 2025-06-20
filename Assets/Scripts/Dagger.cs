using UnityEngine;

[CreateAssetMenu(fileName = "New Dagger", menuName = "Weapons/Dagger")]
public class Dagger : Weapons
{
    public override void Attack(RaycastHit hit)
    {
        base.Attack(hit);
        Debug.Log("Dagger attack executed!");
    }
}
