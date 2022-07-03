using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFuncObject
{
    public Action<GameObject, GameObject> interact;

    public void doFunc(GameObject a,GameObject b)
    {
        interact?.Invoke(a,b);
    }
}
