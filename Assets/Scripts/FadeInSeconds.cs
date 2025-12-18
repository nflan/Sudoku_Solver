using System.Collections;
using TMPro;
using UnityEngine;

public class FadeInSeconds : MonoBehaviour
{
    [Header("FadeInSeconds informations")]
    [SerializeField] private float fadeTime = 0;
    [SerializeField] private TMP_Text text;


    public void Fade()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        float waitTime = 0;
        while (waitTime < 1)
        {
            text.fontMaterial.SetColor("_FaceColor", Color.Lerp(Color.white, Color.clear, waitTime));
            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }
        this.gameObject.SetActive(false);
    }
}
