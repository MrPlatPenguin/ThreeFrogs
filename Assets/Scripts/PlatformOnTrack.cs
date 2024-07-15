using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformOnTrack : MonoBehaviour
{
    public int FrogsOnPlatform { get => frogsOnPlatform.Count; }
    [SerializeField] Transform platform;
    [SerializeField] float alpha;
    [SerializeField] float halfHeight;
    List<GameObject> frogsOnPlatform = new List<GameObject>();
    public void SetAlpha(float alpha)
    {
        float yOffset = Mathf.Lerp(-halfHeight, halfHeight, alpha);
        platform.localPosition = Vector3.up * yOffset;
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out FrogController frog) || frogsOnPlatform.Contains(collision.gameObject) || frog.CurrentState != FrogController.EFrogState.Idle)
            return;

        frogsOnPlatform.Add(collision.gameObject);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out FrogController frog))
            return;

        frogsOnPlatform.Remove(collision.gameObject);
    }

    public void AddFrog()
    {
        frogsOnPlatform.Add(null);
    }

    public void RemoveFrog()
    {
        frogsOnPlatform.Remove(null);
    }

}
