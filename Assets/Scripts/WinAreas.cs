using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinAreas : MonoBehaviour
{
    [SerializeField] BoxCollider2D[] triggers;
    [SerializeField] FrogManager frogManager;
    [SerializeField] GameObject winUI;
    [SerializeField] PauseSystem pauseSystem;
    public void CheckWin()
    {
        int frogsInWinLocations = 0;
        foreach (BoxCollider2D trigger in triggers)
        {
            bool hit = Physics2D.OverlapBox(trigger.bounds.center, trigger.bounds.size, 0, LayerMask.GetMask("Player"));
            if (hit)
                frogsInWinLocations++;
        }
        if (frogsInWinLocations == 3)
        {
            WinLevel();
        }
    }

    void WinLevel()
    {
        Debug.Log("Win");
        winUI.SetActive(true);
        frogManager.Disable();
        pauseSystem.UnPause();
        pauseSystem.gameObject.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
