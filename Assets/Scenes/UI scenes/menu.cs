using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class menu : MonoBehaviour
{
    [SerializeField]private GameObject inicio;
    [SerializeField]private GameObject opciones;
    [SerializeField]private GameObject creditos;
    [SerializeField]private VideoPlayer video;
    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        inicio.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(false);
        video.loopPointReached += Finalizado;
    }

    private void Finalizado(VideoPlayer source)
    {
        inicio.SetActive(true);
    }

    public void Play()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Settings()
    {
        inicio.SetActive(false);
        opciones.SetActive(true);
    }

    public void Return()
    {
        inicio.SetActive(true);
        opciones.SetActive(false);
        creditos.SetActive(false);
    }

    public void Credits()
    {
        inicio.SetActive(false);
        creditos.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Sound()
    {

    }

    public void Quality()
    {

    }

    public void Controls()
    {

    }
}
