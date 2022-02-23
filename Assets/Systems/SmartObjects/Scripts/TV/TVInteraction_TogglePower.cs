using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SmartObject_TV))]
public class TVInteraction_TogglePower : SimpleInteraction
{
    protected SmartObject_TV LinkedTV;

    protected void Awake()
    {
        LinkedTV = GetComponent<SmartObject_TV>();
    }

    public override void Perform(MonoBehaviour performer, UnityAction<BaseInteraction> onCompleted)
    {
        LinkedTV.ToggleState();

        base.Perform(performer, onCompleted);
    }
}
