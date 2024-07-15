using System;
using UnityEngine;
using UnityEngine.Events;

public class HangPoint : MonoBehaviour
{
    [SerializeField] GameObject targetSprite;

    public UnityEvent onHang;
    public UnityEvent onRelease;


    public void Target()
    {
        targetSprite.SetActive(true);
    }
    public void Untarget()
    {
        targetSprite.SetActive(false);
    }

    public void Hang()
    {
        onHang?.Invoke();
    }

    public void Release()
    {
        onRelease?.Invoke();
    }
}
