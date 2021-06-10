using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//future upgrades:
//upgrade this to read specific input without needing to set a unity input
//handle multiple input
//ahndle muliple input variations like left right, get key up, etc.

public class InputBasedTestingScript : MonoBehaviour
{


    [SerializeField] private string inputKeyName;
    public UnityEvent functionsToTest;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(inputKeyName)) {
            functionsToTest?.Invoke();
        }
    }
}
