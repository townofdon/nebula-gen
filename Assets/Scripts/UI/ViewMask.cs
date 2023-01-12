using UnityEngine;

public class ViewMask : ViewBase
{
    protected override void OnActivate()
    {
        nebula2.GenerateNoise();
        spriteBackground.SetActive(false);
        spriteOutput.SetActive(false);
        spriteDrawSurface.SetActive(false);
        spriteNoise.SetActive(true);
    }
}