using UnityEngine;

public class ViewMain : ViewBase
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
