using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Gestiona las referencias a los puntos de anclaje del jugador (arma y hitbox).
    /// </summary>
    public class PlayerPointSwitcher : MonoBehaviour
    {
        [Header("Puntos de arma")]
        public Transform WeaponPoint;
        public Transform WeaponPoint1;
        [Header("Puntos de hitbox")]
        public Transform HitBoxPoint;
        public Transform HitBoxPoint1;

        /// <summary>
        /// Activa/desactiva los GameObjects de los puntos correctos según la cámara.
        /// </summary>
        public void UpdateActivePoints(bool isCameraInverted)
        {
            if (WeaponPoint != null) WeaponPoint.gameObject.SetActive(!isCameraInverted);
            if (HitBoxPoint != null) HitBoxPoint.gameObject.SetActive(!isCameraInverted);
            if (WeaponPoint1 != null) WeaponPoint1.gameObject.SetActive(isCameraInverted);
            if (HitBoxPoint1 != null) HitBoxPoint1.gameObject.SetActive(isCameraInverted);
        }
    }
}
