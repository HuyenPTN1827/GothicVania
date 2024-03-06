using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Enemy Boss;
    public GameObject creditsScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Boss == null)
        {
            StartCoroutine(ShowCredits());
        }
    }



    private IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(2f);
        creditsScreen.SetActive(true) ;

        yield return new WaitForSeconds(2f);
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
