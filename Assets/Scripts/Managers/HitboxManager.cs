using UnityEngine;
using ProyectSecret.Utils;
using ProyectSecret.Combat.Behaviours;
using System.Collections.Generic;

namespace ProyectSecret.Managers
{
    public class HitboxManager : MonoBehaviour
    {
        public static HitboxManager Instance { get; private set; }

        // Usamos el prefab del GameObject como clave para el pool.
        private readonly Dictionary<GameObject, ObjectPool<WeaponHitbox>> _hitboxPools = new Dictionary<GameObject, ObjectPool<WeaponHitbox>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public WeaponHitbox Get(GameObject hitboxPrefab)
        {
            if (!_hitboxPools.ContainsKey(hitboxPrefab))
            {
                // Si no existe un pool para este prefab, lo creamos sobre la marcha.
                var pool = new ObjectPool<WeaponHitbox>(hitboxPrefab, 5, transform);
                _hitboxPools[hitboxPrefab] = pool;
            }

            var hitboxObject = _hitboxPools[hitboxPrefab].Get();
            var hitbox = hitboxObject.GetComponent<WeaponHitbox>();
            hitbox.Pool = _hitboxPools[hitboxPrefab]; // Asignamos el pool para que pueda devolverse.
            return hitbox;
        }
    }
}