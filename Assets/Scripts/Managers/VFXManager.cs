using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectSecret.Utils;
using ProyectSecret.Events;

namespace ProyectSecret.VFX
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }
        
        [System.Serializable]
        public class VfxPoolConfig
        {
            public string Key;
            public GameObject Prefab;
            public int InitialSize = 10;
        }
        
        [Header("Configuración de Pools de VFX")]
        [SerializeField] private List<VfxPoolConfig> _vfxPoolsConfig;
        
        private Dictionary<string, ObjectPool<PooledParticleSystem>> _vfxPools;
        
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
                InitializePools();
                // Suscribirse al evento para reproducir VFX
                GameEventBus.Instance?.Subscribe<HitboxImpactEvent>(OnHitboxImpact);
                GameEventBus.Instance?.Subscribe<PlayVFXRequest>(OnPlayVFXRequest);
            }
        }

        private void InitializePools()
        {
            _vfxPools = new Dictionary<string, ObjectPool<PooledParticleSystem>>();
            foreach (var config in _vfxPoolsConfig)
            {
                if (config.Prefab == null)
                {
                    Debug.LogWarning($"VFXManager: El prefab para la clave '{config.Key}' no está asignado.");
                    continue;
                }

                if (config.Prefab.GetComponent<PooledParticleSystem>() == null)
                {
                    Debug.LogError($"VFXManager: El prefab para la clave '{config.Key}' no tiene el componente 'PooledParticleSystem'.");
                    continue;
                }

                var pool = new ObjectPool<PooledParticleSystem>(config.Prefab, config.InitialSize, transform);
                _vfxPools[config.Key] = pool;
            }
        }

        private void OnDestroy()
        {
            // Buena práctica: desuscribirse para evitar errores.
            if (Instance == this && GameEventBus.Instance != null)
            {
                GameEventBus.Instance.Unsubscribe<HitboxImpactEvent>(OnHitboxImpact);
                GameEventBus.Instance.Unsubscribe<PlayVFXRequest>(OnPlayVFXRequest);
            }
        }

        /// <summary>
        /// Obtiene un efecto del pool y lo activa en la posición y rotación deseadas.
        /// </summary>
        public GameObject PlayEffect(string key, Vector3 position, Quaternion? rotation = null)
        {
            if (!_vfxPools.ContainsKey(key))
            {
                Debug.LogWarning($"VFXManager: No se encontró un pool para la clave '{key}'.");
                return null;
            }
            
            GameObject vfxObject = _vfxPools[key].Get();
            if (vfxObject == null) return null;

            var vfxInstance = vfxObject.GetComponent<PooledParticleSystem>();
            if (vfxInstance == null)
            {
                Debug.LogError($"VFXManager: El objeto del pool para la clave '{key}' no tiene el componente 'PooledParticleSystem'. El prefab está mal configurado.");
                // Devolver el objeto al pool si es posible o simplemente desactivarlo.
                vfxObject.SetActive(false); 
                return null;
            }
            
            vfxInstance.Pool = _vfxPools[key]; // Asumo que PooledParticleSystem tiene esta propiedad.

            vfxInstance.transform.position = position;
            vfxInstance.transform.rotation = rotation ?? Quaternion.identity;
            vfxObject.SetActive(true);
            return vfxObject;
        }

        private void OnPlayVFXRequest(PlayVFXRequest request)
        {
            PlayEffect(request.Key, request.Position, request.Rotation);
        }

        private void OnHitboxImpact(HitboxImpactEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.WeaponData.ImpactVFXKey))
            {
                PlayEffect(evt.WeaponData.ImpactVFXKey, evt.ImpactPoint);
            }
        }

        public void PlayFadeAndDestroyEffect(GameObject targetObject, float duration)
        {
            // Esta lógica es más específica y podría ir en su propio componente,
            // pero por ahora la mantenemos aquí para simplicidad.
            StartCoroutine(FadeRoutine(targetObject, duration));
        }

        private IEnumerator FadeRoutine(GameObject targetObject, float duration)
        {
            // Aquí podrías añadir una lógica de fade más compleja si lo necesitas.
            // Por ahora, simplemente desactivamos el objeto después de la duración.
            yield return new WaitForSeconds(duration);
            
            // En lugar de destruir, lo desactivamos. Si es un objeto del pool,
            // su propio componente PooledParticleSystem lo devolverá.
            // Si no, simplemente se desactiva.
            if (targetObject != null)
            {
                GameEventBus.Instance.Publish(new VFXCompletedEvent(targetObject));
                targetObject.SetActive(false);
            }
        }
    }
}
