using System.Collections.Generic;
using UnityEngine;

namespace ProyectSecret.Utils
{
    /// <summary>
    /// Una clase genérica y reutilizable para gestionar un pool de GameObjects.
    /// </summary>
    /// <typeparam name="T">El tipo de componente que se espera que tengan los objetos del pool.</typeparam>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly GameObject prefab;
        private readonly List<GameObject> pooledObjects;
        private readonly Transform parentTransform;

        /// <summary>
        /// Crea un nuevo pool de objetos.
        /// </summary>
        /// <param name="prefab">El prefab que se usará para instanciar los objetos.</param>
        /// <param name="initialSize">El número de objetos a crear inicialmente.</param>
        /// <param name="parent">Un transform opcional para agrupar los objetos del pool en la jerarquía.</param>
        public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parentTransform = parent;
            pooledObjects = new List<GameObject>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// Obtiene un objeto inactivo del pool.
        /// </summary>
        /// <returns>Un GameObject inactivo o null si no hay ninguno disponible.</returns>
        public GameObject Get()
        {
            foreach (var obj in pooledObjects)
            {
                if (obj != null && !obj.activeInHierarchy)
                {
                    return obj;
                }
            }

            // Opcional: se podría expandir el pool dinámicamente aquí si se necesita.
            Debug.LogWarning($"El pool para {prefab.name} se ha quedado sin objetos. Considere aumentar el tamaño del pool.");
            return null;
        }

        /// <summary>
        /// Devuelve un objeto al pool, desactivándolo.
        /// </summary>
        /// <param name="obj">El objeto a devolver.</param>
        public void Return(GameObject obj)
        {
            if (obj != null)
            {
                obj.transform.SetParent(parentTransform); // Devuelve el objeto al contenedor del pool.
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// Destruye todos los objetos del pool y lo vacía.
        /// Es importante llamar a este método cuando el pool ya no se necesite para liberar memoria.
        /// </summary>
        public void Clear()
        {
            foreach (var obj in pooledObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj);
                }
            }
            pooledObjects.Clear();
        }

        private GameObject CreateNewObject()
        {
            GameObject newObj = Object.Instantiate(prefab, parentTransform);
            newObj.SetActive(false);
            pooledObjects.Add(newObj);
            return newObj;
        }
    }
}
