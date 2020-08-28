using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnder : GridObject
{

    public void Enter() {

        GameController.instance.NextLevel();
        
    }

}
