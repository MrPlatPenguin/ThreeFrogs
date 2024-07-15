using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tigger : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onEnter?.Invoke();
        }
    }
}
