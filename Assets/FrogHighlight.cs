using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogHighlight : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float startSize;
    [SerializeField] float endSize;
    [SerializeField] float totalTime;
    [SerializeField] SpriteRenderer sr;
    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float a = curve.Evaluate(Mathf.Clamp01(t / totalTime));
        transform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, a);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1- a);
        if (a >= 1)
            Destroy(gameObject);
    }
}
