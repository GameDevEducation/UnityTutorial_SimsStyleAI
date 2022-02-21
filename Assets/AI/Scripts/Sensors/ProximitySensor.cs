using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseAI))]
public class ProximitySensor : MonoBehaviour
{
    BaseAI LinkedAI;
    LocalDetectableTargetManager TargetManager;

    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<BaseAI>();
        TargetManager = GetComponent<LocalDetectableTargetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        List<DetectableTarget> targets = TargetManager != null ? TargetManager.AllTargets : DetectableTargetManager.Instance.AllTargets;

        for (int index = 0; index < targets.Count; ++index)
        {
            var candidateTarget = targets[index];

            // skip if ourselves
            if (candidateTarget.gameObject == gameObject)
                continue;

            if (Vector3.Distance(LinkedAI.EyeLocation, candidateTarget.transform.position) <= LinkedAI.ProximityDetectionRange)
                LinkedAI.ReportInProximity(candidateTarget);
        }
    }
}
