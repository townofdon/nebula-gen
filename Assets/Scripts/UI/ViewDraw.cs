using UnityEngine;

public class ViewDraw : ViewBase
{
    protected override void OnActivate()
    {
        nebula2.DrawOutput();
        spriteBackground.SetActive(false);
        spriteOutput.SetActive(false);
        spriteDrawSurface.SetActive(true);
        spriteNoise.SetActive(true);

        spriteNoise.SetAlpha(0.6f);
    }
}