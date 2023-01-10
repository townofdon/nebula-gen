using UnityEngine;

public class ViewMask : ViewBase
{
    protected override void OnActivate()
    {
        nebula2.DrawOutput();
        spriteBackground.SetActive(false);
        spriteOutput.SetActive(false);
        spriteDrawSurface.SetActive(false);
        spriteNoise.SetActive(true);
    }
}