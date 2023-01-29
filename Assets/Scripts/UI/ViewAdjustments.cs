using UnityEngine;

public class ViewAdjustments : ViewBase
{
    protected override void OnActivate()
    {
        nebula2.GenerateNoise();
        nebula2.DrawOutput();
        spriteBackground.SetActive(true);
        spriteOutput.SetActive(true);
        spriteDrawSurface.SetActive(false);
        spriteNoise.SetActive(false);
        spriteMask.SetActive(false);
    }
}