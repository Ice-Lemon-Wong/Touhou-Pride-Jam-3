using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ActSequnceManager : MonoBehaviour
{
    [SerializeField] PlayableDirector pd;
    [SerializeField] TimelineAsset timeline;

    // Start is called before the first frame update
    void Start()
    {
        pd.Play(timeline);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
