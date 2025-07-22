using UnityEngine;
using System.IO;
using ProyectSecret.Combat.SceneManagement;
using Newtonsoft.Json; // Usaremos la librería Newtonsoft.Json

namespace ProyectSecret.Managers
{
    /// <summary>
    /// Gestiona el guardado y la carga del estado del juego en un archivo JSON.
    /// </summary>
    public static class SaveLoadManager
    {
        private static readonly string saveFileName = "savegame.json";

        // Configuraciones para la serialización con Newtonsoft.Json.
        // TypeNameHandling.Auto es crucial para que pueda manejar herencia (diferentes tipos de items en una lista).
        // ReferenceLoopHandling.Ignore evita problemas con referencias circulares, comunes en Unity.
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private static string GetSaveFilePath()
        {
            // Application.persistentDataPath es una carpeta segura en cualquier plataforma (PC, Mac, Android, etc.)
            // para guardar datos de usuario.
            return Path.Combine(Application.persistentDataPath, saveFileName);
        }

        /// <summary>
        /// Guarda el estado actual del juego en un archivo.
        /// </summary>
        /// <param name="data">Los datos persistentes del jugador a guardar.</param>
        public static void SaveGame(PlayerPersistentData data)
        {
            if (data == null)
            {
                Debug.LogError("SaveLoadManager: Se intentó guardar con datos nulos.");
                return;
            }

            // Usamos Newtonsoft.Json para convertir el objeto a una cadena de texto JSON.
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, jsonSettings);
            
            string filePath = GetSaveFilePath();
            File.WriteAllText(filePath, json);

            #if UNITY_EDITOR
            Debug.Log($"Juego guardado en: {filePath}");
            #endif
        }

        /// <summary>
        /// Carga el estado del juego desde un archivo.
        /// </summary>
        /// <param name="data">El ScriptableObject donde se cargarán los datos.</param>
        /// <returns>True si la carga fue exitosa, false si no se encontró el archivo.</returns>
        public static bool LoadGame(PlayerPersistentData data)
        {
            if (data == null)
            {
                Debug.LogError("SaveLoadManager: Se intentó cargar en un objeto de datos nulo.");
                return false;
            }

            string filePath = GetSaveFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                // Sobrescribimos los datos del ScriptableObject en memoria con los del archivo usando Newtonsoft.
                JsonConvert.PopulateObject(json, data, jsonSettings);
                
                // Después de cargar, marcamos que hay una posición guardada para que el inicializador la use.
                data.HasSavedPosition = true;
                // Y nos aseguramos de que no esté en estado de "derrota".
                data.CameFromDefeat = false;

                #if UNITY_EDITOR
                Debug.Log($"Juego cargado desde: {filePath}");
                #endif
                return true;
            }
            else
            {
                #if UNITY_EDITOR
                Debug.LogWarning($"Archivo de guardado no encontrado en: {filePath}");
                #endif
                return false;
            }
        }

        /// <summary>
        /// Comprueba si existe un archivo de guardado.
        /// </summary>
        public static bool SaveFileExists()
        {
            return File.Exists(GetSaveFilePath());
        }
    }
}
