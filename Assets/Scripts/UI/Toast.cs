using System.Collections;
using UnityEngine;

public class Toast : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] float messageDisplayTime = 2f;
    [SerializeField] float fadeTime = 2f;

    Coroutine hiding;

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvas.enabled = true;
        canvasGroup.alpha = 1f;

        if (hiding != null) StopCoroutine(hiding);
        hiding = StartCoroutine(HideAfter(messageDisplayTime));
    }

    void Hide()
    {
        canvas.enabled = false;
        hiding = null;
    }

    IEnumerator HideAfter(float duration)
    {
        yield return new WaitForSeconds(duration);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Max(canvasGroup.alpha, 0f);
            yield return new WaitForEndOfFrame();
        }

        Hide();
    }
}
