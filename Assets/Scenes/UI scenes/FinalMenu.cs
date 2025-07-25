using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalMenu : MonoBehaviour
{
    private menu menu;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Introduccion");
    }

    public void ReturnGame()
    {
        SceneManager.LoadScene("Exploracion");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Introduccion");
        menu.FromVictory();
    }
}
