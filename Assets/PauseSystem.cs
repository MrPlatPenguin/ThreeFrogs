using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] FrogManager frogManager;
    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        paused = !paused;

        if (!paused)
        {
            UnPause();
        }
        else
            Pause();
    }

    public void UnPause()
    {
        paused = false;
        frogManager.Enable();
        pauseUI.SetActive(false);
    }

    public void Pause()
    {
        paused = true;
        frogManager.Disable();
        pauseUI.SetActive(true);
    }
}
