using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static bool isLevelComplete = false;
    public GameObject continueButton;
    public static LevelManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        isLevelComplete = false;

        if (continueButton != null)
            continueButton.SetActive(false);
    }

    public void CheckCompletion()
    {
        if (isLevelComplete) return;
        if (GridManager.Instance == null) return;

        bool[] grid = GridManager.Instance.occupied;

        for (int i = 0; i < grid.Length; i++)
        {
            if (!grid[i])
                return;
        }

        isLevelComplete = true;

        if (continueButton != null)
            continueButton.SetActive(true);

        Debug.Log("LEVEL COMPLETE!");
    }


    public void gotolvl1()
    {
        SceneManager.LoadScene(0);
    }
    public void gotolvl2()
    {
        SceneManager.LoadScene(1);
    }
    public void gotolvl3()
    {
        SceneManager.LoadScene(2);
    }
    public void gotolvl4()
    {
        SceneManager.LoadScene(3);
    }
    public void gotolvl5()
    {
        SceneManager.LoadScene(4);
    }
    public void gotolvl6()
    {
        SceneManager.LoadScene(5);
    }
    public void gotolvl7()
    {
        SceneManager.LoadScene(6);
    }
    public void gotolvl8()
    {
        SceneManager.LoadScene(7);
    }
    public void gotolvl9()
    {
        SceneManager.LoadScene(8);
    }
    public void gomainmenu()
    {
        SceneManager.LoadScene(9);
    }

    public void golevels()
    {
        SceneManager.LoadScene(10);
    }

}