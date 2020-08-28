using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : GridObject
{

    public static PlayerScript instance;

    [SerializeField]private int life = 3;
    public int Life {
        get {
            return life;
        }
        set {

            life = Mathf.Clamp(value, 0, 4);

            for(int i = 0; i < lifeIcons.Length; i++) {

                if(i < life) {

                    lifeIcons[i].enabled = true;

                } else {

                    lifeIcons[i].enabled = false;

                }

            }

        }
    }

    [SerializeField]private Image[] lifeIcons;

    [SerializeField]private int score = 0;
    public int Score {
        get {
            return score;
        }
        set {

            if(value > 0)
                score = value;
            else
                score = 0;

            scoreboard.text = "" + score;

        }
    }

    [SerializeField]private Text scoreboard; 

    [SerializeField]private Vector2Int lookDirection;
    public Animator _animator;
    public SpriteRenderer _renderer;

    private Coroutine damageEffect;

    private void Start() {

        if(instance != null) {

            Debug.Log("PlayerScript: More than one instance of this script was found!");
            return;

        }

        instance = this;

        _animator = GetComponentInChildren<Animator>();
        if(_animator == null)
            Debug.Log("PlayerScript: No Animator found!");

        _renderer = GetComponentInChildren<SpriteRenderer>();
        if(_renderer == null)
            Debug.Log("SpriteRenderer: No Animator found!");

        this.gameObject.SetActive(false);

    }

    void OnEnable() {    

        lookDirection = new Vector2Int(0,-1);
        UpdateLookDirection();

    }

    public void InputX(int dir)
    {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(_animator.GetBool("attack"))
            return;

        if(lookDirection.x != dir) {

            lookDirection = new Vector2Int(dir, 0);
            UpdateLookDirection();

        } else {

            if(gridPosition.x + dir >= 0 && gridPosition.x + dir < 6) {

                GridObject node = GridScript.instance.GetNodeContent(new Vector2Int(dir, 0) + gridPosition);
                if(node == null) {

                    GridScript.instance.SwapNodes(new Vector2Int(dir, 0) + gridPosition, gridPosition);
                    AudioController.instance.Play("walk");

                } else if(node.GetType() == typeof(LevelExit)){
                    
                    Interact(node);
                    
                } else if(node.GetType() == typeof(LifeScript) || node.GetType() == typeof(KeyScript)){

                    GridScript.instance.SwapNodes(new Vector2Int(dir, 0) + gridPosition, gridPosition);
                    Interact(node);

                } else {

                    Interact(node);

                }

            }

        }

    }

    public void InputY(int dir)
    {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(_animator.GetBool("attack"))
            return;

        if(lookDirection.y != dir) {

            lookDirection = new Vector2Int(0, dir);
            UpdateLookDirection();
           

        } else {

            if(gridPosition.y + dir >= 0 && gridPosition.y + dir < 4) {

                GridObject node = GridScript.instance.GetNodeContent(new Vector2Int(0, dir) + gridPosition);
                if(node == null) {

                    GridScript.instance.SwapNodes(new Vector2Int(0, dir) + gridPosition, gridPosition);
                    AudioController.instance.Play("walk");

                } else if(node.GetType() == typeof(LevelExit)){
                    
                    Interact(node);
                    
                } else if(node.GetType() == typeof(LifeScript) || node.GetType() == typeof(KeyScript)){

                    GridScript.instance.SwapNodes(new Vector2Int(0, dir) + gridPosition, gridPosition);
                    Interact(node);

                } else {

                    Interact(node);

                }

            }

        }

    }

    private void Interact(GridObject obj) {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(obj.GetType() == typeof(LevelExit)){

            LevelExit exit = (LevelExit)obj;
            exit.EnterDoor();

        } else if(obj.GetType() == typeof(LifeScript)) {

            LifeScript life = (LifeScript)obj;
            life.PickUp();

        } else if(obj.GetType() == typeof(DamageNode)) {

            DamageNode damage = (DamageNode)obj;
            damage.DamagePlayer();

        } else if(obj.GetType() == typeof(Enemy) || obj.GetType() == typeof(PatternEnemy)) {

            Enemy enemy = (Enemy)obj;
            enemy.DamagePlayer();

        } else if(obj.GetType() == typeof(KeyScript)) {

            KeyScript key = (KeyScript)obj;
            key.PickUp();

        } else if(obj.GetType() == typeof(GameEnder)) {

            GameEnder end = (GameEnder)obj;
            end.Enter();

        }

    }

    private void UpdateLookDirection() {

        _animator.SetInteger("look_x", Mathf.RoundToInt(lookDirection.x));
        _animator.SetInteger("look_y", Mathf.RoundToInt (lookDirection.y));

    }

    public void Attack() {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(_animator.GetBool("attack"))
            return;

        _animator.SetBool("attack", true);
        AudioController.instance.Play("attack");

        Vector2Int dif = gridPosition + lookDirection;

        if(dif.x >= 0 && dif.y >= 0 && dif.x < 6 && dif.y < 4 ) {

            GridObject node = GridScript.instance.GetNodeContent(dif);

            if(node != null && (node.GetType() == typeof(Enemy) || node.GetType() == typeof(PatternEnemy))) {

                Enemy enemy = (Enemy)node;
                enemy.TakeDamage();

            }

        }

    }

    public void Damage() {

        if(GameController.instance.currentGameState != GameController.GameState.Gameplay)
            return;

        if(Life - 1 > 0) {
            AudioController.instance.Play("damage");

            if(damageEffect != null) {

                StopCoroutine(damageEffect);
                damageEffect = null;
                _renderer.enabled = true;

            }

            damageEffect = StartCoroutine(DamageEffect());
        }

        PlayerScript.instance.Life--;

        if(life == 0)
            Die();

    }

    IEnumerator DamageEffect() {
        
        _renderer.enabled = false;

        yield return new WaitForFixedUpdate();

        for(int i = 0; i < 4; i++) {

            float time = 0;
            while(time < 0.2f) {

                if(time < 0.1f)
                    _renderer.enabled = false;
                else
                    _renderer.enabled = true;

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

            }

        }

    }

    private void Die() {

        if(damageEffect != null) {

            StopCoroutine(damageEffect);
            damageEffect = null;

        }

        AudioController.instance.Play("die");
        _renderer.enabled = false;
        GameController.instance.currentGameState = GameController.GameState.Restart;
        StartCoroutine(RestartGame());

    }

    IEnumerator RestartGame() {

        float time = 0;
        while(time < 1) {

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

        }

        GameController.instance.SetFade(true);

        time = 0;
        while(time < 1.2f) {

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

        }

        SceneManager.LoadScene(0);

    }

}
