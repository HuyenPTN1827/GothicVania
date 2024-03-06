using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Click");
    }

    public void Quit()
    {
        Application.Quit();

    }
}
