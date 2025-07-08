using UnityEngine;

namespace ProyectSecret.Core
{
    public class DayNightCycle : MonoBehaviour
    {
        [SerializeField] private Light directionalLight;
        [SerializeField] private float dayDuration = 60f; // Duración del día en segundos
        [SerializeField] private float nightDuration = 120f; // Duración de la noche en segundos (el doble)

        private float timer = 0f;
        private bool isDay = true;

        void Start()
        {
            if (directionalLight == null)
            {
                directionalLight = FindObjectOfType<Light>();
            }
            SetDay();
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (isDay && timer >= dayDuration)
            {
                SetNight();
            }
            else if (!isDay && timer >= nightDuration)
            {
                SetDay();
            }
            RotateLight();
        }

        void SetDay()
        {
            isDay = true;
            timer = 0f;
            if (directionalLight != null)
            {
                directionalLight.intensity = 1f;
                directionalLight.color = Color.white;
            }
        }

        void SetNight()
        {
            isDay = false;
            timer = 0f;
            if (directionalLight != null)
            {
                directionalLight.intensity = 0.2f;
                directionalLight.color = new Color(0.2f, 0.2f, 0.5f);
            }
        }

        void RotateLight()
        {
            if (directionalLight != null)
            {
                float totalDuration = isDay ? dayDuration : nightDuration;
                float anglePerSecond = 180f / totalDuration;
                directionalLight.transform.Rotate(Vector3.right, anglePerSecond * Time.deltaTime);
            }
        }
    }
}
