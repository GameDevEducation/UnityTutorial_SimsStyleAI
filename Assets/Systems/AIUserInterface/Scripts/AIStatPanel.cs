using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIStatPanel : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI StatName;
    [SerializeField] protected Slider StatValue;

    protected AIStat LinkedStat;

    public void Bind(AIStat stat, float initialValue)
    {
        LinkedStat = stat;
        StatName.text = LinkedStat.DisplayName;
        StatValue.SetValueWithoutNotify(initialValue);
    }

    public void OnStatChanged(float newValue)
    {
        StatValue.SetValueWithoutNotify(newValue);
    }
}
