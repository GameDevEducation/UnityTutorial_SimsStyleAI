using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectableTarget : MonoBehaviour
{
    [SerializeField] bool LocalTargetManagerOnly = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!LocalTargetManagerOnly)
            DetectableTargetManager.Instance.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (!LocalTargetManagerOnly && DetectableTargetManager.Instance != null)
            DetectableTargetManager.Instance.Deregister(this);
    }
}
