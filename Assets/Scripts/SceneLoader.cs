using ProyectSecret.Combat.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    private InputAction interact;
    CombatSceneLoader combatLoader;
    void Awake()
    {
        interact = InputSystem.actions.FindAction("Interact");
        combatLoader = GetComponent<CombatSceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interact.WasPressedThisFrame())
        {
            //combatLoader.LoadCombatScene(playerPrefab, enemyPrefab, kryptoniteItemId, playerInstance);
        }
    }
}
