using UnityEngine;

public class FieldPersistence : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        // yeah, this code smells stanky, like real bad. however, believe it or not, this was the path of least resistance.
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.persistence;
        }
        else
        {
            return nebula2.noiseOptionsB.persistence;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {

            nebula2.noiseOptionsA.persistence = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.persistence = incoming;
        }
        AfterChange();
    }
}
