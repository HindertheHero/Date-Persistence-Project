using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
#if Unity_Editor
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    public TMP_InputField NameInput;

    private const string PlayerDataFile = "playerdata.json";

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
    }

    // starts game when Start button is pressed
    public void StartGame()
    {
        // readinput, fallback to "Anonymous"
        string playerName = NameInput != null ? NameInput.text.Trim() : string.Empty;
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        // serialize to JSON and write to persistent data path
        var data = new PlayerData { playerName = playerName };
        string json = JsonUtility.ToJson(data);

        string path = Path.Combine(Application.persistentDataPath, PlayerDataFile);
        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"Start clicked — PlayerName saved to JSON: '{playerName}' (path: {path})");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save player data JSON: {ex.Message}");
        }

        SceneManager.LoadScene("Main");
    }
}

