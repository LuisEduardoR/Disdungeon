using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GridObject
{

    protected enum Stance { Passive, PathAgressive, NearAgressive };
    [SerializeField]protected Stance stance = Stance.PathAgressive;

    protected int score = 10;

    [SerializeField]protected Vector2Int lookDirection;
    [SerializeField]protected Animator _animator;

    protected virtual void Start() 
    {

        _animator = GetComponentInChildren<Animator>();
        if(_animator == null)
            Debug.Log("PatternEnemy: No Animator found!");

    }

    protected virtual void OnEnable() {

        GameController.instance.OnTick += OnTick;

    }

    protected virtual void OnTick() {}

    protected virtual bool MoveX(int dir)
    {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return false;

        if(lookDirection.x != dir) {

            lookDirection = new Vector2Int(dir, 0);
            UpdateLookDirection();
            return false;

        } else {

            if(gridPosition.x + dir >= 0 && gridPosition.x + dir < 6) {

                GridObject node = GridScript.instance.GetNodeContent(new Vector2Int(dir, 0) + gridPosition);
                if(node == null) {

                    GridScript.instance.SwapNodes(new Vector2Int(dir, 0) + gridPosition, gridPosition);

                } else if(node.GetType() == typeof(PlayerScript)){
                    
                    if(stance == Stance.PathAgressive || stance == Stance.NearAgressive) {
                        Attack();
                    }
                    return false;
                    
                }

                return true;
            }

            return false;
        }
    }

    protected virtual bool MoveY(int dir)
    {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return false;

        if(lookDirection.y != dir) {

            lookDirection = new Vector2Int(0, dir);
            UpdateLookDirection();
            return false;           

        } else {

            if(gridPosition.y + dir >= 0 && gridPosition.y + dir < 4) {

                GridObject node = GridScript.instance.GetNodeContent(new Vector2Int(0, dir) + gridPosition);
                if(node == null) {

                    GridScript.instance.SwapNodes(new Vector2Int(0, dir) + gridPosition, gridPosition);

                } else if(node.GetType() == typeof(PlayerScript)){
                    
                    if(stance == Stance.PathAgressive || stance == Stance.NearAgressive) {
                        Attack();
                    }
                    return false;
                    
                }

                return true;
            }

            return false;
        }
    }

    protected virtual void UpdateLookDirection() {

        _animator.SetInteger("look_x", lookDirection.x);
        _animator.SetInteger("look_y", lookDirection.y);

    }

    protected virtual void Attack() {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(_animator.GetBool("attack"))
            return;

        _animator.SetBool("attack", true);

        DamagePlayer();

    }

    public virtual void TakeDamage()
    {
        
        PlayerScript.instance.Score += score;
        AudioController.instance.Play("kill");
        GridScript.instance.RemoveObject(gridPosition);
        GameController.instance.OnTick -= OnTick;
        Destroy(gameObject);

    }

    public virtual void DamagePlayer()
    {
        
        PlayerScript.instance.Damage();

    }

}
