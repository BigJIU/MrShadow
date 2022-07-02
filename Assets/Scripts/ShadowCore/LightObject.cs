using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("collision enter!");
        if (col.gameObject.CompareTag("Light"))
        {
            //Into the Light range, inform the Manager to update shadow list
            Debug.Log(transform.name + " enter "+col.transform.name);
            ShadowManager.getInstance.addShadowList(transform);
            return;
        }
        else if (col.gameObject.CompareTag("LO"))
        {
            
            return;
        }
        throw new NotImplementedException();
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Light"))
        {
            //Out of the Light, inform the Manager to update the shadow list
            Debug.Log(transform.name + " exit "+other.transform.name);
            ShadowManager.getInstance.reverseShadow(transform);
            return;
        }
        else if (other.gameObject.CompareTag("LO"))
        {
            
            return;
        }
        throw new NotImplementedException();
    }
}
