using UnityEngine;

public enum NoiseLayer
{
    A,
    B,
}

public abstract class NoiseLayerFieldBase : FieldBase
{

    [SerializeField] protected NoiseLayer noiseLayer;
}
