using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    public float horizontalHeight = -180f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //FIXME: NOTDONE SOFAR
        Debug.Log("collision enter shadow!");
        if (collision.collider.gameObject.CompareTag("Light"))
        {
            //Into the Light range, inform the Manager to update shadow list
            Debug.Log(transform.name + " enter "+collision.transform.name);
            ShadowManager.getInstance.addShadowList(collision.transform);
            return;
        }
        else if (collision.collider.gameObject.CompareTag("LO"))
        {
            
            return;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Light"))
        {
            //Out of the Light, inform the Manager to update the shadow list
            Debug.Log(transform.name + " exit "+collision.transform.name);
            ShadowManager.getInstance.reverseShadow(transform);
            return;
        }
        else if (collision.collider.gameObject.CompareTag("LO"))
        {
            
            return;
        }

    }
}