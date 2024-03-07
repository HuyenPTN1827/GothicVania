using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void NewGame()
    {
        audioManager.PlaySfx(audioManager.clickClip);
        SceneManager.LoadScene(1);
    }

    public void ReturnMenu()
    {
        audioManager.PlaySfx(audioManager.clickClip);
        SceneManager.LoadScene(0);
        Debug.Log("Click");
    }

    public void Quit()
    {
        audioManager.PlaySfx(audioManager.clickClip);
        Application.Quit();

    }
}
