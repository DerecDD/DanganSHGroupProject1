using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public GameObject LoadingScreen;
    public GameObject enableObject; // Assign a GameObject to enable

    public void LoadScene(int sceneId)
    {
        DisableOtherGameObjects();
        EnableGameObject();
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    void DisableOtherGameObjects()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && obj != enableObject) // Disable everything except the assigned object
            {
                obj.SetActive(false);
            }
        }
    }

    void EnableGameObject()
    {
        if (enableObject != null)
        {
            enableObject.SetActive(true);
        }
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        LoadingScreen.SetActive(true);

        yield return new WaitForSeconds(5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
    }
}
