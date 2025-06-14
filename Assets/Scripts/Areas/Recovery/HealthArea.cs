using UnityEngine;
using Base;

public class HealthArea : MonoBehaviour
{
    [Tooltip("Cantidad de vida curada al entrar en el área")]
    public int healAmount = 10;
    [Tooltip("Intervalo en segundos para aplicar curación continua")]
    public float healInterval = 1f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"HealthArea: OnTriggerEnter con {other.gameObject.name}");
        var statHolder = other.GetComponent<BaseStatHolder>();
        if (statHolder == null)
        {
            Debug.LogWarning($"HealthArea: {other.gameObject.name} no tiene BaseStatHolder.");
            return;
        }
        var health = statHolder.Health;
        if (health != null)
        {
            health.AffectValue(healAmount);

            // Evita duplicados
            var existingTimer = other.GetComponent<AreaHealingTimer>();
            if (existingTimer == null)
            {
                var timer = other.gameObject.AddComponent<AreaHealingTimer>();
                timer.Init(healAmount, healInterval);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"HealthArea: OnTriggerExit con {other.gameObject.name}");
        var health = other.GetComponent<BaseStatHolder>().Health;
        if (health != null)
        {
            var timer = other.GetComponent<AreaHealingTimer>();
            if (timer != null)
            {
                Destroy(timer);
                Debug.Log($"HealthArea: {other.gameObject.name} ha salido del área de curación. Curación continua detenida.");
            }
        }
    }
}