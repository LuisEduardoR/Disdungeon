using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public void InputPadRight() {
        if(PlayerScript.instance.gameObject.activeSelf)
            PlayerScript.instance.InputX(1); 
    }
    public void InputPadLeft() {
        if(PlayerScript.instance.gameObject.activeSelf)
            PlayerScript.instance.InputX(-1);
    }
    public void InputPadUp() {
        if(PlayerScript.instance.gameObject.activeSelf)
        PlayerScript.instance.InputY(1);
    }
    public void InputPadDown() {
        if(PlayerScript.instance.gameObject.activeSelf)
            PlayerScript.instance.InputY(-1);
    }
    public void InputAttack() {
        if(PlayerScript.instance.gameObject.activeSelf)
            PlayerScript.instance.Attack();
    }

}
