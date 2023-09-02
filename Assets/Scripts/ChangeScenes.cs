using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelect;

    public static int level = 5;


    private void Start()
    {
        if (GameManager.instance == null)
            AudioManager.instance.Play("Menu");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReturnToMenu()
    {
        SceneTransition.instance.Transition(LoadMenu);
    }

    public void Play()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseLevel(int levelChosen)
    {
        if (levelChosen == -1)
        {
            levelChosen = Random.Range(1, 6);
        }

        level = levelChosen;

        SceneTransition.instance.Transition(LoadGameScene);
    }


    public void Quit()
    {
        Application.Quit();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void LoadMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
