using UnityEngine;
using System.Collections.Generic;

namespace ProyectSecret.Spawning
{
    /// <summary>
    /// Instancia el prefab de la daga en 2 spawn points aleatorios de los asignados en el inspector.
    /// </summary>
    public class DaggerSpawnManager : MonoBehaviour
    {
        [Header("Prefab de la daga")]
        [SerializeField] private GameObject daggerPrefab;
        [Header("Spawn Points (asignar 5 en el inspector)")]
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
        [Header("Cantidad de dagas a instanciar")]
        [SerializeField] private int daggersToSpawn = 2;

        void Start()
        {
            if (daggerPrefab == null || spawnPoints.Count < daggersToSpawn)
            {
                #if UNITY_EDITOR
                Debug.LogWarning("Configura el prefab y al menos tantos spawn points como dagas a instanciar.");
                #endif
                return;
            }
            var selectedIndices = new HashSet<int>();
            while (selectedIndices.Count < daggersToSpawn)
            {
                int idx = Random.Range(0, spawnPoints.Count);
                selectedIndices.Add(idx);
            }
            foreach (int idx in selectedIndices)
            {
                Instantiate(daggerPrefab, spawnPoints[idx].position, spawnPoints[idx].rotation);
            }
        }
    }
}
