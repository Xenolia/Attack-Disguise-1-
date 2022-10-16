using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadGame : MonoBehaviour
{
    [SerializeField] float loadTime=3;
    private void Start()
    {
        StartCoroutine(LoadGameNum());
    }
    IEnumerator LoadGameNum()
    {
        yield return new WaitForSeconds(loadTime);
        SceneManager.LoadScene(1);
    }
}
