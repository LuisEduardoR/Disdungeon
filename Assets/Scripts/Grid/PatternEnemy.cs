using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternEnemy : Enemy
{

    protected int curMove;
    [SerializeField] protected Vector2Int[] moves;

    protected override void OnEnable() {

        base.OnEnable();
        curMove = 0;

    }

    protected override void OnTick() {

        if(stance == Stance.NearAgressive) {

            if(gridPosition.x + 1 < 6){

                Vector2Int dif = new Vector2Int(1, 0);
                GridObject node = GridScript.instance.GetNodeContent(gridPosition + dif);

                if(node != null && node.GetType() == typeof(PlayerScript)) {
                    if(lookDirection != dif) {
                        lookDirection = dif;
                        UpdateLookDirection();
                    } else {
                        Attack();
                    }
                    return;
                }

            } 
            
            if (gridPosition.x - 1 > 0) {

                Vector2Int dif = new Vector2Int(-1, 0);
                GridObject node = GridScript.instance.GetNodeContent(gridPosition + dif);

                if(node != null && node.GetType() == typeof(PlayerScript)) {
                    if(lookDirection != dif) {
                        lookDirection = dif;
                        UpdateLookDirection();
                    } else {
                        Attack();
                    }
                    return;
                }

            } 
            
            if(gridPosition.y + 1 < 4) {

                Vector2Int dif = new Vector2Int(0, 1);
                GridObject node = GridScript.instance.GetNodeContent(gridPosition + dif);

                if(node != null && node.GetType() == typeof(PlayerScript)) {
                    if(lookDirection != dif) {
                        lookDirection = dif;
                        UpdateLookDirection();
                    } else {
                        Attack();
                    }
                    return;
                }

            } 
            
            if (gridPosition.y - 1 > 0) {

                Vector2Int dif = new Vector2Int(0, -1);
                GridObject node = GridScript.instance.GetNodeContent(gridPosition + dif);

                if(node != null && node.GetType() == typeof(PlayerScript)) {
                    if(lookDirection != dif) {
                        lookDirection = dif;
                        UpdateLookDirection();
                    } else {
                        Attack();
                    }
                    return;
                }
            }
        }

        if(moves == null || moves.Length == 0)
            return;


        curMove = curMove % moves.Length;

        bool moved = false;
        if(moves[curMove].x != 0) {

            moved = MoveX(moves[curMove].x);

        } else if (moves[curMove].y != 0) {

            moved = MoveY(moves[curMove].y);

        }

        if(moved)
            curMove++;

    }
}
