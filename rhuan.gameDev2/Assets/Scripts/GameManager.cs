using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public EnemySpawner[] spawners;
    public TextMeshProUGUI goalText;
    public AudioSource audioSource;
    public AudioClip victorySFX;
    public string victoryScene = "Victory";
    public float victoryDelay = 3;
    private int zonesConquered = 0;

    void Start()
    {
        for(int i = 0; i < spawners.Length; i++) spawners[i].gameManager = this;
        goalText.text = "Zones Conquered: 0/" + spawners.Length;
    }

    public void RegionCleared()
    {
        zonesConquered++;
        goalText.text = "Zones Conquered: " + zonesConquered + "/" + spawners.Length;
        if (zonesConquered >= spawners.Length)
        {
            if(victorySFX) audioSource.PlayOneShot(victorySFX);
            Invoke(nameof(LoadVictory), victoryDelay);
        }
    }

    public void LoadVictory() => SceneManager.LoadScene(victoryScene);
}