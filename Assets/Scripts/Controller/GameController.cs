using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    // Singleton to the GameController.
    public static GameController instance;

    // Variables for controlling the game state.
    public enum GameState {
        
        StartScreen, Gameplay, Cutscene, Restart, End

    }
    public GameState currentGameState = GameState.StartScreen;
    private bool stateTransition = true;

    // Objects related to the different game states.
    public GameObject[] startScreenObjects;
    public GameObject[] gameplayObjects;
    public GameObject[] cutsceneObjects;
    public Text cutsceneText;

    // Level control.
    private int currentLevel;
    [SerializeField] private LevelSettings[] levels;

    // Main camera for the game.
    private Camera mainCamera;

    // Variables for controlling the background color.
    private int curColor;
    [SerializeField]private List<Color> backgroundColors;

    // Image used for fading the screen.
    [SerializeField]private Image fadeEffect;

    // Handles Tick based events;
    public delegate void Tick();
    public event Tick OnTick;
    private Coroutine tickClock;

    private void Start () {

        // Sets up a singleton.
        if(instance != null) {

            Debug.Log("GameController: More than one instance of this script was found!");
            return;

        }
        instance = this;

        // Sets up the game.
        StartCoroutine(GameSetup());

    }

    // Sets the game up.
    private IEnumerator GameSetup() {

        // Sets up the camera.
        mainCamera = Camera.main;
        if(mainCamera == null) {

            Debug.Log("GameController: MainCamera not found!");

        }

        // Gets a random color for the backgroud.
        curColor = Random.Range(0, backgroundColors.Count - 1);

        if(mainCamera != null)
            mainCamera.backgroundColor = backgroundColors[curColor];

        // Gives a chance for other scripts to load before continuing.
        yield return new WaitForEndOfFrame();

        DeactivateLevelObjects();
        for(int i = 0; i < startScreenObjects.Length; i++)
            startScreenObjects[i].SetActive(true);

        // Starts fading out the black screen.
        SetFade(false);   

    }

    // Starts the game from the first level.
    public void StartGame() {
        
        PlayerScript.instance.Life = 1;
        PlayerScript.instance.Score = 0;

        currentLevel = -1;
        NextLevel();        

    }

    private void EndGame() {

        currentGameState = GameState.End;
        SetFade(true);

    }

    // Gets the next background color.
    private void NextBackgroundColor() {

        curColor++;
        curColor = curColor % backgroundColors.Count;
        mainCamera.backgroundColor = backgroundColors[curColor];

    }

    // Goes to the next level.
    public void NextLevel() {

        currentLevel++;

            if(currentLevel < levels.Length) {
            // Sets the new game state
            stateTransition = true;
            if(levels[currentLevel].GetType() == typeof(GameplaySettings)) {

                currentGameState = GameState.Gameplay;

            } else if(levels[currentLevel].GetType() == typeof(CutsceneSettings)) {

                currentGameState = GameState.Cutscene;    

            } 

            // Start the level transition.
            SetFade(true);

        } else {
            EndGame();
        }

    }

    // Fades the black screen in and out.
    public void SetFade(bool state) {

        if(state) {

            StartCoroutine(FadeIn());

        } else {

            StartCoroutine(FadeOut());

        }

    }

    // Fades in the black screen over a time.
    private IEnumerator FadeIn() {

        fadeEffect.gameObject.SetActive(true);

        yield return new WaitForFixedUpdate();

        while(fadeEffect.color.a < 1) {

            Color c = fadeEffect.color;
            c.a += Time.fixedDeltaTime;
            fadeEffect.color = c;

            yield return new WaitForFixedUpdate();

        }

        // Changes the background.
        NextBackgroundColor();

        // If this fade is a state transition 
        if(stateTransition) {

            // Clears the objects from the previous level.
            DeactivateLevelObjects();

            // Immediatly starts fading out again.
            StartCoroutine(FadeOut());
        }

        // Ends the game.
        if(currentGameState == GameState.End)
            SceneManager.LoadScene(0);

    }

    // Fades out the black screen over a time.
    private IEnumerator FadeOut() {
        
        // If in a state transition, initiates the new game state.
        if(stateTransition)
            PrepareGameState();

        yield return new WaitForFixedUpdate();

       while(fadeEffect.color.a > 0){

            Color c = fadeEffect.color;
            c.a -= Time.fixedDeltaTime;
            fadeEffect.color = c;

            yield return new WaitForFixedUpdate();

        }

        fadeEffect.gameObject.SetActive(false);

        // If in a state transition, end it.
        if(stateTransition)
            stateTransition = false;

    }

    // Deactivates all objects so the proper ones can be reactivated to the next level.
    private void DeactivateLevelObjects() {

        if(tickClock != null) {
            StopCoroutine(tickClock);
            tickClock = null;
        }

        for(int i = 0; i < startScreenObjects.Length; i++) {

            startScreenObjects[i].SetActive(false);

        }

        GridScript.instance.ClearGrid();

        for(int i = 0; i < gameplayObjects.Length; i++) {
            gameplayObjects[i].SetActive(false);

        }

        for(int i = 0; i < cutsceneObjects.Length; i++) {
            cutsceneObjects[i].SetActive(false);

        }

    }

    // Prepare the new level.
    private void PrepareGameState() {

        switch (currentGameState) {

            case GameState.StartScreen:

                for(int i = 0; i < startScreenObjects.Length; i++)
                    startScreenObjects[i].SetActive(true);

            break;
            case GameState.Gameplay:

                for(int i = 0; i < gameplayObjects.Length; i++)
                    gameplayObjects[i].SetActive(true);

                tickClock = StartCoroutine(TickClock());

            break;
            case GameState.Cutscene:

                for(int i = 0; i < cutsceneObjects.Length; i++)
                    cutsceneObjects[i].SetActive(true);

            break;

        }

        // Do the apropriated actions based on levle type.
        if (levels[currentLevel].GetType() == typeof(GameplaySettings)) {

                // Builds the gameplay level.
                BuildGameplayLevel((GameplaySettings)levels[currentLevel]);

        } else if (levels[currentLevel].GetType() == typeof(CutsceneSettings)) {

            // Sets the cutscene test.
            CutsceneSettings cutsceneSettings = (CutsceneSettings)levels[currentLevel];

            cutsceneSettings.currentText = 0;
            cutsceneText.text = cutsceneSettings.text[0];
            
        }
    }

    
    private void BuildGameplayLevel(GameplaySettings settings) {

        // Position the player.
        GridScript.instance.InsertObject(PlayerScript.instance, settings.playerPosition);
        PlayerScript.instance.gameObject.SetActive(true);

        // Position the exit.
        GridScript.instance.InsertObject(LevelExit.instance, settings.exitPosition);
        LevelExit.instance.gameObject.SetActive(true);
        LevelExit.instance.SetLock(settings.locked);

        // Instantiate other objects.
        for(int i = 0; i < 4; i++) {
            for(int j = 0; j < 6; j++) {
            
                if(settings.levelGrid != null && settings.levelGrid[j, i] != null) {
                    if(new Vector2Int(j, i) != settings.playerPosition && new Vector2Int(j, i) != settings.exitPosition) {

                        GameObject newObj = Instantiate(settings.levelGrid[j, i], Vector3.zero, new Quaternion());
                        GridScript.instance.InsertObject(newObj.GetComponent<GridObject>(), new Vector2Int(j, i));

                    } else
                        Debug.Log("GameController: Object at (" + j + "," + i + ") from level " + currentLevel + " can't be added to the tile is already occupied by the Player or the LevelExit!.");
                }
            }   
        }

    }

    // Shows the next text from a cutscene or goes to the next level if it's already the last one.
    public void AdvanceCutsceneText() {

        CutsceneSettings cutsceneSettings = levels[currentLevel].GetComponent<CutsceneSettings>();

        if(cutsceneSettings == null) {

            Debug.Log("GameController: LevelSettings at " + currentLevel + "is not a cutscene but something is using it as if it is.");
            return;
        }

        cutsceneSettings.currentText++;
        if(cutsceneSettings.currentText >= cutsceneSettings.text.Length)
            NextLevel();
        else
            cutsceneText.text = cutsceneSettings.text[cutsceneSettings.currentText];

    }

    IEnumerator TickClock() {

        float time = 0;
        while(true) {

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            if(time > 0.5f) {
                if(OnTick != null)
                    OnTick();
                time = 0;
            }

        }

    }

}
