using UnityEngine;

namespace ProyectSecret.MonoBehaviours.Player
{
    /// <summary>
    /// Gestiona el cambio de puntos activos (WeaponPoint/WeaponPoint1 y HitBoxPoint/HitBoxPoint1) y mueve los hijos (arma/hitbox) entre ellos según la cámara.
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
        /// Cambia los padres de arma y hitbox al punto activo según la cámara.
        /// </summary>
        public void SwitchPoints(bool isCameraInverted)
        {
            // Weapon
            Transform fromWeapon = isCameraInverted ? WeaponPoint : WeaponPoint1;
            Transform toWeapon = isCameraInverted ? WeaponPoint1 : WeaponPoint;
            if (fromWeapon != null && toWeapon != null && fromWeapon.childCount > 0)
            {
                Transform weapon = fromWeapon.GetChild(0);
                weapon.SetParent(toWeapon);
                weapon.localPosition = Vector3.zero;
                weapon.localRotation = Quaternion.identity;
            }
            // HitBox
            Transform fromHitBox = isCameraInverted ? HitBoxPoint : HitBoxPoint1;
            Transform toHitBox = isCameraInverted ? HitBoxPoint1 : HitBoxPoint;
            if (fromHitBox != null && toHitBox != null && fromHitBox.childCount > 0)
            {
                Transform hitbox = fromHitBox.GetChild(0);
                hitbox.SetParent(toHitBox);
                hitbox.localPosition = Vector3.zero;
                hitbox.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Activa/desactiva los puntos correctos según la cámara.
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
