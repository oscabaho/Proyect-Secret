using UnityEngine;

[CreateAssetMenu(fileName = "New Sword", menuName = "Weapons/Sword")]
public class Sword : Weapons
{
    

    public override void Attack(RaycastHit hit)
    {
        base.Attack(hit);
        
        Debug.Log("Sword attack executed!");
    }
}
