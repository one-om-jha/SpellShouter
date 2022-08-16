using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveComplete : MonoBehaviour
{   
    void Start()
    {
        StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(0.8f);
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
