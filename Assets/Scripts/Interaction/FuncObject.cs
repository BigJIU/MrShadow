
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FuncObject: MonoBehaviour
{
    [Header("Attach")] 
    public List<UnityEvent> shadowAttach;
    public List<UnityEvent> humanAttach;
    public List<UnityEvent> otherAttach;
    
    [Header("Enter")] 
    public List<UnityEvent> shadowEnter;
    public List<UnityEvent> humanEnter;
    public List<UnityEvent> otherEnter;
    
    [Header("Exit")] 
    public List<UnityEvent> shadowExit;
    public List<UnityEvent> humanExit;
    public List<UnityEvent> otherExit;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("SO"))
        {
            foreach (var var in shadowEnter)
            {
                var?.Invoke();
            }
        }
        else if (col.CompareTag("LO"))
        {
            foreach (var var in humanEnter)
            {
                var?.Invoke();
            }
        }
        else
        {
            foreach (var var in otherEnter)
            {
                var?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SO"))
        {
            foreach (var var in shadowExit)
            {
                var?.Invoke();
            }
        }
        else if (other.CompareTag("LO"))
        {
            foreach (var var in humanExit)
            {
                var?.Invoke();
            }
        }
        else
        {
            foreach (var var in otherExit)
            {
                var?.Invoke();
            }
        }
    }

    public void copyComponent(GameObject target)
    {
        UnityEditorInternal.ComponentUtility.CopyComponent(this.GetComponent<FuncObject>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
    }

    //TODO: Just an example
    public void fadeSelf(float time)
    {
        StartCoroutine("Fade");
    }

    public void debugLog(string a)
    {
        Debug.Log(a);
    }
    
}
