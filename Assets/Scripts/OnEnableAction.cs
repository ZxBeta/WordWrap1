using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableAction : MonoBehaviour
{
    public UnityEvent action;


    private void OnEnable()
    {
        action.Invoke();
    }
}
