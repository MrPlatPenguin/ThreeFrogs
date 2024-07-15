using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FrogButton : MonoBehaviour
{
    public UnityEvent activate;
    public UnityEvent deactivate;
    int activations;
    bool active = false;

    [SerializeField] SpriteAnimator anim;
    [SerializeField] SpriteAnimation on, off;
    [SerializeField] AudioClip pressSound;
    AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    void ChangeActivations(int change)
    {
        activations += change;

        if (activations > 0 == active)
            return;

        active = activations > 0;
        if (active)
        {
            activate?.Invoke();
            anim.SetAnimation(on);
            source.PlayOneShot(pressSound);
        }
        else
        {
            deactivate?.Invoke();
            anim.SetAnimation(off);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChangeActivations(1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ChangeActivations(-1);
    }
}
