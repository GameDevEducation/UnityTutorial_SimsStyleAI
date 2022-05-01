using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackUIPanel : MonoBehaviour
{
    [SerializeField] GameObject StatPanelPrefab;
    [SerializeField] Transform StatRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AIStatPanel AddStat(AIStat linkedStat, float initialValue)
    {
        var newGO = Instantiate(StatPanelPrefab, StatRoot);
        newGO.name = $"Stat_{linkedStat.DisplayName}";
        var statPanelLogic = newGO.GetComponent<AIStatPanel>();
        statPanelLogic.Bind(linkedStat, initialValue);

        return statPanelLogic;
    }
}
