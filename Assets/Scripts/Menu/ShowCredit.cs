using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowCredit : MonoBehaviour
{
    //public Animator animator;
    //public float deadAnimationDuration = 2f;
    public GameObject creditsScreen;

    private void OnDestroy()
    {
        Debug.Log("Destroy");
        StartCoroutine(ShowCredits());
    }

    private IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(2f);
        creditsScreen.SetActive(true);

        yield return new WaitForSeconds(2f);
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

