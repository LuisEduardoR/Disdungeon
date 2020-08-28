using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : GridObject
{

    public static LevelExit instance;
    private bool locked;
    private Animator _animator;

    private void Start() {

        if(instance != null) {

            Debug.Log("LevelExit: More than one instance of this script was found!");
            return;

        }

        instance = this;

        _animator = GetComponent<Animator>();
        if(_animator == null)
            Debug.Log("LevelExit: No Animator found!");

        this.gameObject.SetActive(false);

    }

    public void SetLock(bool state) {

        locked = state;
        _animator.SetBool("locked", state);

    }

    public void Unlock() {

        locked = false;
        _animator.SetBool("locked", false);
        AudioController.instance.Play("key");

    }

    public void EnterDoor() {

        if(locked)
            return;

        AudioController.instance.Play("exit");
        PlayerScript.instance.gameObject.SetActive(false);
        GameController.instance.NextLevel();
        
    }

}
