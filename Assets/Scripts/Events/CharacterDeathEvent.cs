using UnityEngine;

namespace ProyectSecret.Events
{
    /// <summary>
    /// Evento que representa la muerte de una entidad (personaje, enemigo, etc.).
    /// </summary>
    public class CharacterDeathEvent
    {
        public GameObject Entity { get; }

        public CharacterDeathEvent(GameObject entity)
        {
            Entity = entity;
        }
    }
}
