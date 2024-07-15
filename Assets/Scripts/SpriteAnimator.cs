using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteAnimation defaultAnimation;
    private SpriteAnimation currentAnimation;

    int _currentFrame;
    float _timer;

    bool paused;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ResetAnimator();
    }

    void Update()
    {
        if (currentAnimation == null)
            return;

        currentAnimation.Update();

        if (paused)
            return;

        _timer += currentAnimation.unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (_timer >= currentAnimation.FrameRate)
            NextFrame();

    }

    void NextFrame()
    {
        _timer -= currentAnimation.FrameRate;
        _currentFrame++;


        if (_currentFrame == currentAnimation.Frames.Length)
        {
            if (currentAnimation.Loop)
                _currentFrame = 0;
            else
            {
                paused = true;
                return;
            }
        }

        SetFrame(_currentFrame);
    }

    void SetFrame(int index)
    {
        _currentFrame = index;
        spriteRenderer.sprite = currentAnimation.Frames[index];
    }

    /// <summary>
    /// Immediately change to the referenced function.
    /// </summary>
    /// <param name="animationName"></param>
    public void SetAnimation(SpriteAnimation animation)
    {
        if (currentAnimation == animation)
            return;

        currentAnimation.Exit();
        _currentFrame = 0;
        _timer = 0;

        currentAnimation = animation;
        SetFrame(_currentFrame);

        currentAnimation.Enter();
        paused = false;
    }

    public SpriteRenderer GetSpriteRenederer()
    {
        return spriteRenderer;
    }

    public SpriteAnimation GetCurrentAnimation()
    {
        return currentAnimation;
    }

    public string GetCurrentAnimationName()
    {
        return currentAnimation.name;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ResetAnimator()
    {
        currentAnimation = defaultAnimation;

        SetFrame(0);
    }
}

[Serializable]
public class AnimationEventHandler
{
    public int frame;
    public UnityEvent animEvent;
}

[Serializable]
public class AnimationState
{
    public SpriteAnimation animation;
    public AnimationEventHandler[] events;
    [HideInInspector]
    public string name;
}