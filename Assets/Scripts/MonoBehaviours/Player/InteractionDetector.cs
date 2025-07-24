using UnityEngine;
using ProyectSecret.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ProyectSecret.MonoBehaviours.Player
{
    [RequireComponent(typeof(Collider))]
    public class InteractionDetector : MonoBehaviour
    {
        private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();

        private void Awake()
        {
            // Ensure the collider is a trigger
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // DEBUG: ¿Está colisionando con algo?
            Debug.Log($"[Detector] Trigger Enter con: {other.name}", other.gameObject);

            if (other.TryGetComponent<IInteractable>(out var interactable) && !interactablesInRange.Contains(interactable))
            {
                // DEBUG: ¿Ha encontrado un IInteractable?
                Debug.Log($"[Detector] IInteractable AÑADIDO: {other.name}", other.gameObject);
                interactablesInRange.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // DEBUG: ¿Está saliendo de la colisión?
            Debug.Log($"[Detector] Trigger Exit con: {other.name}", other.gameObject);
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                Debug.Log($"[Detector] IInteractable ELIMINADO: {other.name}", other.gameObject);
                interactablesInRange.Remove(interactable);
            }
        }

        public IInteractable GetClosestInteractable(Transform relativeTo)
        {
            // Clean up any null references that might have occurred (e.g., destroyed objects)
            interactablesInRange.RemoveAll(item => item == null || (item as MonoBehaviour) == null);

            return interactablesInRange
                .OrderBy(interactable => Vector3.Distance(relativeTo.position, (interactable as MonoBehaviour).transform.position))
                .FirstOrDefault();
        }
    }
}
