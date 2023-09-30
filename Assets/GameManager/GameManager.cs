using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool m_applicationStarted = false;

    uint m_score; //The score of the current game
    uint m_carrots; //The number of carrots the player has collected in a game
    public Cinemachine.CinemachineVirtualCamera m_virtualCamera; //Main Camera
    public bool m_gameStarted { get; private set; } = false; //Whether the player has started the game
    public bool m_paused { get; private set; } = false; //Whether the game is currently paused

    [Header("Level")]
    public LevelSegment[] m_levelSegments; //A collection of level segments that will be generated as the game progresses
    [SerializeField] Transform m_gameWorld; //The transform in which all the objects in the game world will be stored
                                            //(Used so that all objects inside can be pushed back when the player goes too far ahead)
    [SerializeField] float m_loopDistance; //The max distance the player can go before they are pushed back
                                           //to prevent the player from going too far from origin
    public LevelSegment m_titleLevelSegment; //The level segment in the title screen
    
    [Header("Death Wall")]
    [SerializeField] Transform m_deathWall;
    [SerializeField] float m_deathWallMoveSpeed;
    [SerializeField] float m_deathWallMaxMoveSpeed;
    [SerializeField] float m_deathWallAcceleration;
    [SerializeField] float m_furthestDistance; //The furthest distance the death wall can be from the player

    [Header("UI")]
    [SerializeField] Canvas m_canvas;
    [SerializeField] TMPro.TMP_Text m_scoreUI; //The text that displays the current score in a game
    [SerializeField] TMPro.TMP_Text m_carrotsUI; //The text that displays the amount of carrots collected in a game
    [SerializeField] RectTransform m_pauseScreen;
    [SerializeField] RectTransform m_gameOverScreen;
    [SerializeField] TMPro.TMP_Text m_gameOverScoreUI; //The text that displays the current score of a game in the game over screen
    [SerializeField] TMPro.TMP_Text m_gameOverHighScoreUI; //The text that displays the highscore in the game over screen
    [SerializeField] TMPro.TMP_Text m_gameOverNewHighScoreUI; //The text that displays the new highscore in the game over screen
    public Image m_screenFlash; //The screen flash animation when the player dies
    [SerializeField] AudioSource m_musicSource; //The audio source where the music is from

    void Awake()
    {
        //Load the game when the application has started
        if (!m_applicationStarted) { m_applicationStarted = true; SaveSystem.Load(); }
    }

    void Update()
    {
        //Dont update game if it has not started
        if (!m_gameStarted) return;

        //If game over screen is active press space to restart the game
        if (m_gameOverScreen.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space)) Restart();
        }

        //Pause/Unpause the game
        if (Input.GetKeyDown(KeyCode.Escape)) if (!m_paused) Pause(); else UnPause();

        //Move Death Wall
        m_deathWall.transform.position += Vector3.right * m_deathWallMoveSpeed * Time.deltaTime;
        m_deathWallMoveSpeed += m_deathWallAcceleration * Time.deltaTime * Time.deltaTime;
        if (m_deathWallMoveSpeed > m_deathWallMaxMoveSpeed) m_deathWallMoveSpeed = m_deathWallMaxMoveSpeed;
        
        //Prevent the death wall from getting too far off screen
        float cameraLeftWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
        if (cameraLeftWorldPosition - m_deathWall.position.x - m_furthestDistance > 0)
        {
            m_deathWall.position = new Vector3(cameraLeftWorldPosition - m_furthestDistance, m_deathWall.position.y, m_deathWall.position.z);
        }

        //Prevent the death wall from going too far to the right
        if (m_deathWall.position.x > 1000.0f) m_deathWall.position = new Vector2(1000.0f, m_deathWall.position.y);

        //Set the sprite mask of the death wall
        {
            SpriteRenderer deathWallSprite = m_deathWall.GetChild(0).GetComponent<SpriteRenderer>();
            deathWallSprite.GetComponent<SpriteMask>().sprite = deathWallSprite.sprite;
            
            Vector2 newSpriteSize = deathWallSprite.size;
            newSpriteSize.x = deathWallSprite.transform.position.x + 20.0f;
            deathWallSprite.size = newSpriteSize;
        }

        //Shift objects back if the camera goes further than loopDistance
        if (Camera.main.transform.position.x > m_loopDistance)
        {
            //Shift all objects in world back by loopDistance
            foreach (Transform worldtransforms in m_gameWorld)
            {
                worldtransforms.position -= Vector3.right * m_loopDistance;
            }
        
            //Shift all particles in the world back by loopDistance
            foreach (ParticleSystem particleSystem in m_gameWorld.GetComponentsInChildren<ParticleSystem>())
            {
                if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local) continue;
        
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
                particleSystem.GetParticles(particles);
        
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].position -= Vector3.right * m_loopDistance;
                }
        
                particleSystem.SetParticles(particles);
            }
        
        
            //Shift virtualCamera in world back by loopDistance
            m_virtualCamera.ForceCameraPosition(m_virtualCamera.transform.position -= Vector3.right * m_loopDistance, Quaternion.identity);
            FindObjectOfType<ParallaxBackground>().ClearLastCameraPosition();
        }
    }

    void OnApplicationPause(bool _pause)
    {
        if (!_pause) return;

        //Pause the game when the application is not in focus
        if (!m_paused && m_gameStarted) Pause();
    }

    void OnApplicationQuit()
    {
        //Save the game when the player exits the application
        SaveSystem.Save();
    }

    public void Pause()
    {
        if (m_gameOverScreen.gameObject.activeSelf) return;

        m_paused = true;
        m_pauseScreen.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void UnPause()
    {
        if (m_gameOverScreen.gameObject.activeSelf) return;

        m_paused = false;
        m_pauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void triggerGameOver()
    {
        //Update High Score
        if (m_score > SaveSystem.m_data.m_highScore)
        {
            //Update Score UI and HighScore
            m_gameOverScoreUI.gameObject.SetActive(false);
            m_gameOverHighScoreUI.gameObject.SetActive(false);
            m_gameOverNewHighScoreUI.gameObject.SetActive(true);
            m_gameOverNewHighScoreUI.text = "NEW HIGH SCORE!!!\n" + (SaveSystem.m_data.m_highScore = (int)m_score);
        }
        else
        {
            //Update Score UI
            m_gameOverScoreUI.text = "Score: " + m_score.ToString();
            m_gameOverHighScoreUI.text = "High Score: " + SaveSystem.m_data.m_highScore.ToString();
        }

        //Update Carrots
        SaveSystem.m_data.m_carrots += m_carrots;

        //Open GameOver Screen
        m_gameOverScreen.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        m_canvas.gameObject.SetActive(true);
        m_musicSource.Play();
        m_gameStarted = true;
    }

    public void GoToTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        GoToTitle();
        Menus.m_playOnAwake = true;
    }

    public void ShareHighScore()
    {
        //This function takes a screenshot and shares it. Used for sharing highscores.

        IEnumerator TakeScreenshotAndShare()
        {
            yield return new WaitForEndOfFrame();

#if UNITY_ANDROID || UNITY_IOS
            //Takes the screen shot
            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
            File.WriteAllBytes(filePath, screenShot.EncodeToPNG());

            //Destroy screenshot to avoid memory leaks
            Destroy(screenShot);

            new NativeShare().AddFile(filePath)
                .SetSubject("Awesome score from Overflow")
                .SetText(SaveSystem.m_data.m_highScore.ToString() + " points!" + "I'm on a roll!")
                .SetUrl("https://birdbraingamesdev.itch.io/").Share();
#endif
        }

        TakeScreenshotAndShare();
    }

    public void AddScore(uint _score)
    {
        m_score += _score;
        m_scoreUI.text = m_score.ToString();
    }

    public void AddCarrot(uint _carrots)
    {
        m_carrots += _carrots;
        m_carrotsUI.text = m_carrots.ToString();
    }
}
