using System.Collections;
using UnityEngine;

namespace ProyectSecret.Enemies
{
    public class EnemyAttackController : MonoBehaviour
    {
        public AttackPhase CurrentPhase => currentPhase;
        public enum AttackPhase { Phase1, Phase2, Phase3 }
        [Header("Fase actual")]
        [SerializeField] private AttackPhase currentPhase = AttackPhase.Phase1;

        [Header("Fase 1: Caída de piedras")]
        [SerializeField] private GameObject rockPrefab;
        [SerializeField] private int rocksToSpawn = 5;
        [SerializeField] private float rockSpawnHeight = 10f;
        [SerializeField] private float rockSpawnRadius = 5f;
        [SerializeField] private float rockSpawnInterval = 0.5f;

        [Header("Fase 2: Parte vulnerable")]
        [SerializeField] private GameObject vulnerablePartPrefab;
        [SerializeField] private float vulnerablePartDuration = 3f;
        [SerializeField] private Transform vulnerableSpawnPoint;

        [Header("Fase 3: Carga y ataque recto")]
        [SerializeField] private float chargeTime = 2f;
        [SerializeField] private float attackSpeed = 10f;
        [SerializeField] private float attackDuration = 1.5f;
        private Vector3 attackDirection;
        private Vector3 targetPosition;

        private Coroutine phaseRoutine;
        private EnemyHealthController healthController;

        private void Awake()
        {
            // Cacheamos la referencia a la salud del enemigo para pasarla a la parte vulnerable.
            healthController = GetComponent<EnemyHealthController>();
        }

        public void StartPhase1()
        {
            currentPhase = AttackPhase.Phase1;
            if (phaseRoutine != null) StopCoroutine(phaseRoutine);
            phaseRoutine = StartCoroutine(Phase1Routine());
        }

        public void StartPhase2()
        {
            currentPhase = AttackPhase.Phase2;
            if (phaseRoutine != null) StopCoroutine(phaseRoutine);
            phaseRoutine = StartCoroutine(Phase2Routine());
        }

        public void StartPhase3(Transform player)
        {
            currentPhase = AttackPhase.Phase3;
            if (phaseRoutine != null) StopCoroutine(phaseRoutine);
            phaseRoutine = StartCoroutine(Phase3Routine(player));
        }

        private IEnumerator Phase1Routine()
        {
            // Enemigo invulnerable, lanza piedras
            for (int i = 0; i < rocksToSpawn; i++)
            {
                Vector3 spawnPos = transform.position + Vector3.up * rockSpawnHeight + Random.insideUnitSphere * rockSpawnRadius;
                GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
                // El script RockController se encarga de destruir la piedra al colisionar con el suelo
                yield return new WaitForSeconds(rockSpawnInterval);
            }
        }

        private IEnumerator Phase2Routine()
        {
            // Enemigo ataca con parte vulnerable
            GameObject partObject = Instantiate(vulnerablePartPrefab, vulnerableSpawnPoint.position, Quaternion.identity);
            
            // Inyectamos la dependencia de la salud del enemigo.
            var vulnerableController = partObject.GetComponent<VulnerablePartController>();
            if (vulnerableController != null)
            {
                vulnerableController.Initialize(healthController);
            }
            else
            {
                #if UNITY_EDITOR
                Debug.LogWarning("El prefab de la parte vulnerable no tiene el componente VulnerablePartController.");
                #endif
            }

            yield return new WaitForSeconds(vulnerablePartDuration);
            if (partObject != null) Destroy(partObject);
        }

        private IEnumerator Phase3Routine(Transform player)
        {
            // Enemigo carga ataque apuntando al jugador
            float timer = 0f;
            while (timer < chargeTime)
            {
                targetPosition = player.position;
                attackDirection = (targetPosition - transform.position).normalized;
                timer += Time.deltaTime;
                yield return null;
            }
            // Ataque en línea recta hacia la última posición del jugador
            float attackTimer = 0f;
            while (attackTimer < attackDuration)
            {
                transform.position += attackDirection * attackSpeed * Time.deltaTime;
                attackTimer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
