using NUnit.Framework.Internal.Commands;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class menu : MonoBehaviour
{
    [Header("Inicio")]
    [SerializeField]private GameObject inicio;
    [Header("Opciones")]
    [SerializeField]private GameObject opciones;
    [SerializeField]private GameObject MusicOptions;
    [SerializeField]private GameObject GraficSettings;
    [Header("Creditos")]
    public GameObject creditos;
    [Header("Video inicio")]
    public VideoPlayer video;
    public InputAction skip;
    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        inicio.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(false);
        video.loopPointReached += Finalizado;
        skip = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if (skip.WasPressedThisFrame())
        {
            SkipVideo();
        }
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
        MusicOptions.SetActive(true);
    }

    public void Quality()
    {
        GraficSettings.SetActive(true);
    }

    public void Controls()
    {

    }

    public void Back()
    {
        MusicOptions.SetActive(false);
        GraficSettings.SetActive(false);
    }

    public void SkipVideo()
    {
        video.Stop();
        Finalizado(video);
    }

    public void FromVictory()
    {
        video.Stop();
        creditos.SetActive(true);
    }
}
