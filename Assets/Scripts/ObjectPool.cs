using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool
{
    GameObject prototype;
    List<GameObject> pool;
    bool canGrow;
    int nectCheck;

    public ObjectPool(GameObject prototype, bool resizable, int size)
    {
        this.prototype = prototype;
        this.canGrow = resizable;
        this.pool = new List<GameObject>();
        for(int i = 0; i < size; i++)
        {
            GameObject o = UnityEngine.Object.Instantiate(prototype);
            //can't just call instantiate bc monobehavior
            o.SetActive(false);
            pool.Add(o);
        }
    }

    public GameObject GetObject()
    {
        //find next avaliable object if any
        int started = nectCheck;
        do
        {
            // if this object is avaliable, turn it on and return it
            if (pool[nectCheck].activeSelf)
            {

                GameObject o = pool[nectCheck];
                nectCheck = (nectCheck + 1) % pool.Count;
                o.SetActive(true);
                return o;
            }
            
        }
        while (nectCheck != started);
        //if there weren't any avaiable objects 
        if (canGrow)
        {
            GameObject o = UnityEngine.Object.Instantiate(prototype);
            pool.Add(o);
            return o;
        }
        else
        {
            //there are no avaliable objects and the pool cannot grow
            return null;
        }
       
    }

}
