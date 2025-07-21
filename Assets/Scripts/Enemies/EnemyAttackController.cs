using System.Collections;
using UnityEngine;
using ProyectSecret.Interfaces;
using ProyectSecret.VFX;
using ProyectSecret.Utils; // 1. Importar el namespace del ObjectPool

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
        [SerializeField] private GameObject shadowPrefab; // Prefab para la sombra/indicador
        [SerializeField] private int numberOfGroups = 3;
        [SerializeField] private int rocksPerGroup = 5;
        [SerializeField] private float intervalBetweenGroups = 2f;
        [SerializeField] private float spawnDelayWithinGroup = 0.1f;
        [SerializeField] private float rockSpawnHeight = 10f;
        [SerializeField] private float rockSpawnRadius = 5f; // Radio de caída alrededor del jugador
        [SerializeField] private LayerMask groundLayer; // Capa del suelo para el Raycast
        
        [Header("Fase 1 - Pooling")]
        [SerializeField] private int rockPoolSize = 20;
        [SerializeField] private int shadowPoolSize = 20;

        [Header("Fase 2: Parte vulnerable")]
        [SerializeField] private GameObject vulnerablePartPrefab;
        [SerializeField] private Transform vulnerableSpawnPoint;

        [Header("Fase 3: Carga y ataque recto")]
        [SerializeField] private float chargeTime = 2f;
        [SerializeField] private float attackSpeed = 10f;
        [SerializeField] private float attackDuration = 1.5f;
        private Vector3 attackDirection;
        private Vector3 targetPosition;

        private Coroutine phaseRoutine;
        private EnemyHealthController healthController;
        private ObjectPool<RockController> rockPool;
        private ObjectPool<ShadowController> shadowPool;

        private void Awake()
        {
            // Cacheamos la referencia a la salud del enemigo para pasarla a la parte vulnerable.
            healthController = GetComponent<EnemyHealthController>();

            // Inicializamos los pools de rocas y sombras
            rockPool = new ObjectPool<RockController>(rockPrefab, rockPoolSize, transform);
            if (shadowPrefab != null)
                shadowPool = new ObjectPool<ShadowController>(shadowPrefab, shadowPoolSize, transform);
        }

        public void StartPhase1(Transform player)
        {
            currentPhase = AttackPhase.Phase1;
            if (phaseRoutine != null) StopCoroutine(phaseRoutine);
            phaseRoutine = StartCoroutine(Phase1Routine(player));
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

        private IEnumerator Phase1Routine(Transform player)
        {
            // Enemigo invulnerable, lanza grupos de piedras que caen cerca del jugador.
            for (int g = 0; g < numberOfGroups; g++)
            {
                for (int r = 0; r < rocksPerGroup; r++)
                {
                    // 1. Calcular la posición de spawn de la roca en el cielo
                    Vector3 spawnCenter = player.position;
                    Vector3 spawnPos = spawnCenter + Vector3.up * rockSpawnHeight + Random.insideUnitSphere * rockSpawnRadius;

                    // 2. Lanzar un Raycast hacia abajo para encontrar el punto de impacto en el suelo
                    GameObject shadowInstance = null;
                    if (shadowPool != null && Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 100f, groundLayer))
                    {
                        // 3. Obtener una sombra del pool y posicionarla en el punto de impacto
                        shadowInstance = shadowPool.Get();
                        if (shadowInstance != null)
                        {
                            // Posicionamos la sombra en el suelo y ligeramente elevada para evitar Z-fighting
                            shadowInstance.transform.position = hit.point + new Vector3(0, 0.01f, 0);
                            shadowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                            shadowInstance.SetActive(true);
                        }
                    }

                    // 4. Obtener una roca del pool, posicionarla y activarla
                    GameObject rock = rockPool.Get();
                    if (rock == null) continue;

                    rock.transform.position = spawnPos;
                    rock.transform.rotation = Quaternion.identity;
                    rock.SetActive(true);
                    rock.GetComponent<RockController>()?.Initialize(rockPool, shadowInstance);
                    
                    if (spawnDelayWithinGroup > 0)
                        yield return new WaitForSeconds(spawnDelayWithinGroup);
                }
                // Intervalo entre grupos de rocas.
                yield return new WaitForSeconds(intervalBetweenGroups);
            }
        }

        private IEnumerator Phase2Routine()
        {
            // Enemigo ataca con parte vulnerable. La parte vulnerable se autodestruirá.
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

            // La corutina termina aquí. Ya no es responsable de destruir la parte vulnerable.
            yield break;
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
