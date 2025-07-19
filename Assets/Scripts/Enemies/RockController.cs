using UnityEngine;

namespace ProyectSecret.Enemies
{
    public class RockController : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
