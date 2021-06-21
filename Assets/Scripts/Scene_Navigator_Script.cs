using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;

public class Scene_Navigator_Script : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.2f;
    [SerializeField] private float waitInterval = 1;
    [SerializeField] private GameObject transitionScreen;

    private bool movingScene = false;

    void Start(){

        StartCoroutine(StartScene());
        movingScene = false;

        //if (SceneManager.GetActiveScene().buildIndex == 0)
        //{
        //    StartCoroutine(EnterMainMenu());
        //}
    }
 

    public void AdvenceScene()
    {
        if (movingScene == false)
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToScene(SceneManager.GetActiveScene().buildIndex + 1));
            
            
            movingScene = true;
            
        } 
        
       
    }

    //Call this
    public void GoToScene(int index)
    {
        if (movingScene == false)
        {
            Debug.Log("Next");
            StopAllCoroutines();
            StartCoroutine(TransitionToScene(index));
            
            movingScene = true;
            
        }

      
    }

    IEnumerator StartScene()
    {
        
        yield return new WaitForSecondsRealtime(waitInterval);

		transitionScreen.GetComponent<Animator>().Play("transitioned");

		movingScene = false;
    }

    public void QuitGame()
    {
       
        StartCoroutine(Quit());
        
    }

    IEnumerator Quit()
    {
        transitionScreen.GetComponent<Animator>().Play("transition");
        yield return new WaitForSecondsRealtime(transitionTime+ waitInterval);
        Application.Quit();
    }

    IEnumerator TransitionToScene(int index)
    {
        transitionScreen.GetComponent<Animator>().Play("transition");
        yield return new WaitForSecondsRealtime(waitInterval + transitionTime);
        //FindObjectOfType<Audio_Manager>().UpdateSounds(index);
       
        SceneManager.LoadScene(index);
        
    }

    IEnumerator EnterMainMenu()
    {   
        yield return new WaitForSecondsRealtime(5);
        StartCoroutine(TransitionToScene(SceneManager.GetActiveScene().buildIndex + 1));
        

        movingScene = true;
    }

    public void Click() {
        Debug.Log("click");
    
    }
}
