using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    [Header("Configuración del ciclo")]
    [SerializeField] private Light luzPrincipal; // Luz que simula el sol/luna
    [SerializeField] private float duracionDia = 30f; // Duración del día en segundos
    [SerializeField] private float duracionNoche = 60f; // Duración de la noche en segundos (el doble que el día)

    [Header("Skybox")]
    [SerializeField] private Material skyboxDia;
    [SerializeField] private Material skyboxNoche;

    [Header("Skybox Procedural (opcional)")]
    [SerializeField] private bool usarSkyboxProcedural = false;
    [SerializeField] private Color colorCieloDia = new Color(0.5f, 0.7f, 1f);
    [SerializeField] private Color colorCieloNoche = new Color(0.05f, 0.05f, 0.15f); // Azul muy oscuro

    private float tiempoTranscurrido = 0f;
    private bool esDia = true;
    private Camera camaraPrincipal;
    private float progresoTransicionSkybox = 0f;
    [SerializeField] private float velocidadTransicionSkybox = 0.2f; // Entre 0.01 y 1, menor es más lento

    void Start()
    {
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;

        // Calcula el objetivo de la transición (0 = día, 1 = noche)
        float objetivoTransicion = esDia ? 0f : 1f;
        // Transición suave independiente del ciclo
        progresoTransicionSkybox = Mathf.MoveTowards(progresoTransicionSkybox, objetivoTransicion, velocidadTransicionSkybox * Time.deltaTime);

        if (esDia && tiempoTranscurrido >= duracionDia)
        {
            CambiarANoche();
        }
        else if (!esDia && tiempoTranscurrido >= duracionNoche)
        {
            CambiarADia();
        }

        // Rotación suave de la luz (simulación de sol/luna)
        float progreso = esDia ? (tiempoTranscurrido / duracionDia) : (tiempoTranscurrido / duracionNoche);
        float angulo = esDia ? Mathf.Lerp(0f, 180f, progreso) : Mathf.Lerp(180f, 360f, progreso);
        luzPrincipal.transform.rotation = Quaternion.Euler(angulo, 0f, 0f);

        // Transición de intensidad y color de la luz
        float intensidadDia = 1f;
        float intensidadNoche = 0.15f;
        Color colorDia = Color.yellow;
        Color colorNoche = new Color(0.2f, 0.3f, 0.6f); // Azul oscuro
        luzPrincipal.intensity = Mathf.Lerp(intensidadDia, intensidadNoche, progresoTransicionSkybox);
        luzPrincipal.color = Color.Lerp(colorDia, colorNoche, progresoTransicionSkybox);

        // Cambia el Skybox según el ciclo (instantáneo, pero puedes dejarlo si quieres cambiar el material)
        if (skyboxDia != null && skyboxNoche != null)
        {
            RenderSettings.skybox = progresoTransicionSkybox < 0.5f ? skyboxDia : skyboxNoche;
        }

        // Transición suave del color del Skybox procedural
        if (usarSkyboxProcedural && RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_SkyTint"))
        {
            Color colorActual = Color.Lerp(colorCieloDia, colorCieloNoche, progresoTransicionSkybox);
            RenderSettings.skybox.SetColor("_SkyTint", colorActual);
        }

        // Transición de color de fondo de la cámara
        if (camaraPrincipal != null)
        {
            Color fondoDia = new Color(0.5f, 0.7f, 1f); // Azul claro
            Color fondoNoche = new Color(0.05f, 0.05f, 0.15f); // Azul muy oscuro
            camaraPrincipal.backgroundColor = Color.Lerp(fondoDia, fondoNoche, progresoTransicionSkybox);
        }
    }

    void CambiarANoche()
    {
        esDia = false;
        tiempoTranscurrido = 0f;
    }

    void CambiarADia()
    {
        esDia = true;
        tiempoTranscurrido = 0f;
    }
}
