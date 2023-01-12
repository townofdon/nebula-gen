using UnityEngine;

public class ViewDraw : ViewBase
{
    protected override void OnActivate()
    {
        nebula2.GenerateNoise();
        spriteBackground.SetActive(false);
        spriteOutput.SetActive(false);
        spriteDrawSurface.SetActive(true);
        spriteNoise.SetActive(true);
        spriteMask.SetActive(false);

        spriteNoise.SetAlpha(0.6f);
    }
}