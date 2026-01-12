using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public Text HighScore;

    private bool m_Started = false;
    private int m_Points;
    public int m_HighScore;

    private bool m_GameOver = false;

    private const string HighScoreKey = "HighScore";
    private const string HighScoreNameKey = "HighScoreName";
    private const string PlayerDataFile = "playerdata.json";

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        // initialize UI and load saved high score
        m_Points = 0;
        ScoreText.text = $"Score : {m_Points}";
        LoadHighScore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }


    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        // save high score and associated name if current score is higher
        if (m_Points > m_HighScore)
        {
            m_HighScore = m_Points;
            PlayerPrefs.SetInt(HighScoreKey, m_HighScore);

            // get current player name from JSON saved by Menu (fallback to "Anonymous")
            string currentPlayer = LoadPlayerNameFromJson();
            if (string.IsNullOrEmpty(currentPlayer))
                currentPlayer = "Anonymous";

            PlayerPrefs.SetString(HighScoreNameKey, currentPlayer);
            PlayerPrefs.Save();

            HighScore.text = $"High Score : {m_HighScore} - {currentPlayer}";
        }
    }

    private void LoadHighScore()
    {
        m_HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        string savedName = PlayerPrefs.GetString(HighScoreNameKey, "None");
        HighScore.text = $"High Score : {m_HighScore} - {savedName}";
    }

    // Reads the JSON file written by Menu and returns the saved player name
    private string LoadPlayerNameFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, PlayerDataFile);
        if (!File.Exists(path))
        {
            Debug.Log($"Player data JSON not found at {path}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
                return null;

            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data != null ? data.playerName : null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to read player data JSON: {ex.Message}");
            return null;
        }
    }
}






