using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] private SceneAsset scene;
    public float delaySecond = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SetActive(false);
            NodeSelect();
        }
    }

    public void NodeSelect()
    {
        StartCoroutine((IEnumerator)LoadAfterDelay());
    }

    IEnumerable LoadAfterDelay()
    {
        yield return new WaitForSeconds(delaySecond);
        SceneManager.LoadScene(scene.name);
    }
}
