using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeScript : GridObject
{

    public void PickUp()
    {

        if(PlayerScript.instance.Life < 3)
            PlayerScript.instance.Life++;
        else
            PlayerScript.instance.Score+=50;
        AudioController.instance.Play("powerup");
        GridScript.instance.RemoveObject(gridPosition);
        Destroy(gameObject);

    }

}
