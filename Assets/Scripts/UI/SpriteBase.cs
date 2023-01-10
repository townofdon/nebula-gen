using UnityEngine;

public class SpriteBase : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        SetAlpha(1f);
    }

    public void SetAlpha(float value)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = spriteRenderer.color.toAlpha(value);
    }
}