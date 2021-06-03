using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartAndQuit : MonoBehaviour
{
    public bool pressToQuit = false;
    public bool pressToRestart = false;

    public static RestartAndQuit instanceRnQ;

    private void Awake()
    {
        if (instanceRnQ == null)
        {
            instanceRnQ = this;
        }

        if (instanceRnQ != this)
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && pressToRestart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        // Disabled this for the web build
        // if (Input.GetKeyDown(KeyCode.Escape) && pressToQuit) {
        //     Application.Quit();
        // }
    }

    public void Restart(float duration = 0) {
        Debug.Log("restarting");
        StartCoroutine(RestartScene( duration));
    }

    IEnumerator RestartScene(float duration)
    {
   
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
