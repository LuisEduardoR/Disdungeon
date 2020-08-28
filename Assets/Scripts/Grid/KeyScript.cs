using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : GridObject
{

    public void PickUp()
    {

        LevelExit.instance.Unlock();
        GridScript.instance.RemoveObject(gridPosition);
        Destroy(gameObject);

    }

}
