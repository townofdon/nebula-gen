using UnityEngine;

public class ViewNoise : ViewBase
{
    protected override void OnActivate()
    {
        spriteBackground.SetActive(false);
        spriteOutput.SetActive(false);
        spriteDrawSurface.SetActive(false);
        spriteNoise.SetActive(true);
        spriteMask.SetActive(false);
    }
}