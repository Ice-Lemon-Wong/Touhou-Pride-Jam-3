using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [SerializeField] private Vector3 squashSize;
    [SerializeField] private Vector3 stretchSize;
    [SerializeField] private float transformRate = 10;
    [SerializeField] private bool useRealative;
    //if useRealative is enabled the squash size will be the ratio of the original size 
    //rather than the exact size it's self

    private Vector3 originalSize;
    private Vector3 targetSize;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        targetSize = originalSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //squash and stretch
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, transformRate * Time.deltaTime);
    }

    public void SetToSquash() {
        if (useRealative)
        {
            targetSize = new Vector3(originalSize.x * squashSize.x, originalSize.y * squashSize.y, originalSize.z * squashSize.z);
        }
        else {
            targetSize = squashSize;
        }

    }

    //returns to original size once it is done
    public void SetToSquash(float duration)
    {
        StartCoroutine(SquashTimer(duration));
    }

    public void SetToStretch()
    {
        if (useRealative)
        {
            targetSize = new Vector3(originalSize.x * stretchSize.x, originalSize.y * stretchSize.y, originalSize.z * stretchSize.z);
        }
        else
        {
            targetSize = stretchSize;
        }
    }

    //returns to original size once it is done
    public void SetToStretch(float duration)
    {
        StartCoroutine(StretchTimer(duration));
    }

    public void customSquish(Vector3 newSize, bool useRealitive = false)
    {
        if (useRealitive) {
            targetSize = new Vector3(originalSize.x * newSize.x, originalSize.y * newSize.y, originalSize.z * newSize.z);
        }
        else{
            targetSize = newSize;
        }
        
    }

    //returns to original size once it is done
    public void customSquish(Vector3 newSize, float duration, bool useRealitive = false)
    {
        StartCoroutine(SquishTimer(newSize, duration, useRealitive));
    }
    
    //resets back to the original size
    public void ResetScale()
    {
        targetSize = originalSize;
    }

    //couroutine used
    IEnumerator SquashTimer(float duration)
    {
        SetToSquash();
        yield return new WaitForSeconds(duration);
        ResetScale();
    }
    IEnumerator StretchTimer(float duration)
    {
        SetToStretch();
        yield return new WaitForSeconds(duration);
        ResetScale();
    }
    IEnumerator SquishTimer(Vector3 newSize, float duration, bool useRealitive)
    {

        customSquish(newSize, useRealitive);
        yield return new WaitForSeconds(duration);
        ResetScale();
    }

}
