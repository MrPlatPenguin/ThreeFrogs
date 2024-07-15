using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogManager : MonoBehaviour
{
    public FrogController[] frogs;
    [SerializeField] Transform averagePosition;
    [SerializeField] AudioClip swapSound;
    AudioSource source;
    [SerializeField] FrogHighlight highlight;

    int controlledFrogIndex = 0;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        frogs[0].Posses();
        UpdateAveragePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ControlFrog(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ControlFrog(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ControlFrog(2);

        UpdateAveragePosition();
    }

    void ControlFrog(int index)
    {
        if (frogs[index].Possesed)
            return;
        foreach (FrogController frog in frogs)
        {
            frog.Unposses();
        }
        frogs[index].Posses();
        source.PlayOneShot(swapSound);
        Instantiate(highlight, frogs[index].transform.position, Quaternion.identity);
        controlledFrogIndex = index;
    }

    void UpdateAveragePosition()
    {
        float minX = Mathf.Min(Mathf.Min(frogs[0].transform.position.x, frogs[1].transform.position.x), frogs[2].transform.position.x);
        float maxX = Mathf.Max(Mathf.Max(frogs[0].transform.position.x, frogs[1].transform.position.x), frogs[2].transform.position.x);
        float minY = Mathf.Min(Mathf.Min(frogs[0].transform.position.y, frogs[1].transform.position.y), frogs[2].transform.position.y);
        float maxY = Mathf.Max(Mathf.Max(frogs[0].transform.position.y, frogs[1].transform.position.y), frogs[2].transform.position.y);

        float averageX = (minX + maxX) * 0.5f;
        float averageY = (minY + maxY) * 0.5f;

        averagePosition.position = new Vector2(averageX, averageY);
    }

    public void Disable()
    {
        foreach (FrogController frog in frogs)
        {
            frog.Unposses();
        }
        enabled = false;
    }

    public void Enable()
    {
        enabled = true;
        ControlFrog(controlledFrogIndex);
    }
}
