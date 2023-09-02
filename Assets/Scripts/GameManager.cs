using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public bool ingame;

    public GameObject playerPrefab;
    public Vector2[] startPositions;
    public Animator countdownAnim;
    [HideInInspector] public KeyCode[] controls = new KeyCode[] { KeyCode.LeftShift, KeyCode.X, KeyCode.M, KeyCode.Return };

    public Animator gameOverAnim;
    public TMP_Text winnerText;

    public GameObject ReadyUpPrefab;

    [HideInInspector] public Turret[] playerScripts = new Turret[4];

    public Material[] mats;
    public Color[] colours;

    private int playersRemaining;

    public bool skipStart;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {

        var op = SceneManager.LoadSceneAsync("Map" + ChangeScenes.level, LoadSceneMode.Additive);
        yield return new WaitUntil(() => op.isDone);

        ingame = true;
        AudioManager.instance.Play("Song" + ChangeScenes.level);

        SpawnPlayers();
        playersRemaining = 4;
        SetPlayersCanMove(false);

        if (!skipStart)
        {
            yield return StartCoroutine(ReadyUp());
            countdownAnim.SetTrigger("Countdown");
            yield return new WaitForSeconds(3f);
        }

        SetPlayersCanMove(true);
    }

    private IEnumerator ReadyUp()
    {
        ReadyUp[] readyUps = new ReadyUp[4];
        for (int i = 0; i < 4; i++)
        {
            ReadyUp readyUp = Instantiate(ReadyUpPrefab, Vector2.zero, Quaternion.identity).GetComponent<ReadyUp>();
            readyUp.Setup(i+1);
            readyUps[i] = readyUp;

            if (startPositions[i].y < -1f)
            {
                readyUp.readyCanvas.localPosition = startPositions[i] + Vector2.up * 1.7f;
                readyUp.arrow.localPosition = new Vector2(0f, -60f);
                readyUp.arrow.rotation = Quaternion.identity;
                readyUp.text.localPosition = new Vector2(0f, 60f);
            }
            else
            {
                readyUp.readyCanvas.localPosition = startPositions[i] - Vector2.up * 1.7f; ;
            }
        }
        bool[] playersReady = new bool[4];

        while (!CheckPlayersReady(playersReady))
        {
            for (int i = 0; i < 4; i++)
            {
                if (playerScripts[i].buttonTap)
                {
                    playersReady[i] = true;
                }
                if (playersReady[i] && readyUps[i] != null)
                {
                    Destroy(readyUps[i].gameObject);
                }
            }
            yield return null;
        }
    }
    private bool CheckPlayersReady(bool[] playersReady)
    {
        int amountReady = 0;
        for (int i = 0; i < playersReady.Length; i++)
        {
            if (playersReady[i] == true)
            {
                amountReady++;
            }
        }
        return amountReady == 4;
    }


    private void SpawnPlayers()
    {
        

        for (int i = 0; i < 4; i++)
        {
            startPositions[i] = GameObject.Find("Spawn" + (i + 1)).transform.position;
            Turret playerScript = Instantiate(playerPrefab, startPositions[i], Quaternion.identity).GetComponent<Turret>();

            playerScript.Setup(i + 1);

            playerScripts[i] = playerScript;
        }
    }

    private void SetPlayersCanMove(bool canMove)
    {
        for (int i = 0; i < 4; i++)
        {
            playerScripts[i].SetCanMove(canMove);
        }
    }

    public void PlayerDied()
    {
        playersRemaining--;
        if (playersRemaining == 1)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        SetPlayersCanMove(false);

        string winnerStr = "";
        Turret[] playersLeft = FindObjectsOfType<Turret>();
        foreach (Turret player in playersLeft)
        {
            if (player.isAlive)
            {
                switch (player.playerNum)
                {
                    case 1:
                        winnerStr = "Blue";
                        break;
                    case 2:
                        winnerStr = "Red";
                        break;
                    case 3:
                        winnerStr = "Green";
                        break;
                    case 4:
                        winnerStr = "Pink";
                        break;
                    default:
                        break;
                }
            }
        }
        gameOverAnim.SetTrigger("Game Over");
        winnerText.text = winnerStr + " Wins!";

        AudioManager.instance.StopCurrent();
        StartCoroutine(PlayVictoryMusic());
    }

    private IEnumerator PlayVictoryMusic()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("Victory");
        yield return new WaitForSeconds(4f);
        AudioManager.instance.Play("Post_Victory");
    }

    public void Restart()
    {
        int lastLevel = ChangeScenes.level;

        ChangeScenes.level = Random.Range(1, 6);
        while (ChangeScenes.level == lastLevel)
        {
            ChangeScenes.level = Random.Range(1, 6);
        }

        SceneTransition.instance.Transition(LoadGameScene);
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ShakeCamera(float intensity, float duration)
    {
        CameraShaker.Instance.ShakeOnce(intensity, 10f, duration, duration);
    }
}
