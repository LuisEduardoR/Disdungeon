using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNode : GridObject
{

    public void DamagePlayer()
    {
        
        PlayerScript.instance.Damage();

    }

}
