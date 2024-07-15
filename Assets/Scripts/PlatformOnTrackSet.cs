using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformOnTrackSet : MonoBehaviour
{
    [SerializeField] PlatformOnTrack[] platformOnTracks;

    [SerializeField] float restAlpha = 0f;
    [SerializeField] float platformSpeed = 5f;
    float alpha = 0f;

    private void Awake()
    {
        alpha = restAlpha;
    }

    private void Update()
    {
        alpha = Mathf.MoveTowards(alpha, GetTargetAlpha(), platformSpeed * Time.deltaTime);

        platformOnTracks[0].SetAlpha(alpha);
        platformOnTracks[1].SetAlpha(1 - alpha);
    }

    float GetTargetAlpha()
    {
        // More frogs on platform 1
        if (platformOnTracks[0].FrogsOnPlatform < platformOnTracks[1].FrogsOnPlatform)
            return 1;
        // More frogs on platform 2
        else if (platformOnTracks[0].FrogsOnPlatform > platformOnTracks[1].FrogsOnPlatform)
            return 0;
        // Same frog number on each platform
        else if (platformOnTracks[0].FrogsOnPlatform != 0 && platformOnTracks[0].FrogsOnPlatform == platformOnTracks[1].FrogsOnPlatform)
            return 0.5f;
        // No frog on any platforms
        else
            return restAlpha;
    }
}
