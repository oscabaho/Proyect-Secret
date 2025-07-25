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

        private bool m_IsCameraInverted;

        public Transform ActiveWeaponPoint => m_IsCameraInverted ? WeaponPoint1 : WeaponPoint;
        public Transform ActiveHitBoxPoint => m_IsCameraInverted ? HitBoxPoint1 : HitBoxPoint;

        // Evento que se dispara cuando el punto de anclaje del arma cambia.
        public event System.Action<Transform> OnActiveWeaponPointChanged;

        /// <summary>
        /// Activa/desactiva los GameObjects de los puntos correctos según la cámara.
        /// </summary>
        public void UpdateActivePoints(bool isCameraInverted)
        {
            // Solo actuar si el estado realmente ha cambiado para evitar trabajo innecesario.
            if (m_IsCameraInverted == isCameraInverted) return;

            m_IsCameraInverted = isCameraInverted;

            // Disparar el evento ANTES de desactivar los puntos viejos.
            OnActiveWeaponPointChanged?.Invoke(ActiveWeaponPoint);

            if (WeaponPoint != null) WeaponPoint.gameObject.SetActive(!isCameraInverted);
            if (HitBoxPoint != null) HitBoxPoint.gameObject.SetActive(!isCameraInverted);
            if (WeaponPoint1 != null) WeaponPoint1.gameObject.SetActive(isCameraInverted);
            if (HitBoxPoint1 != null) HitBoxPoint1.gameObject.SetActive(isCameraInverted);
        }
    }
}
