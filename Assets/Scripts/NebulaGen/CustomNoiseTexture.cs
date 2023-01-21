using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

using NebulaGen;
using CyberneticStudios.SOFramework;

public class CustomNoiseTexture : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] FloatVariable _scale;
    [SerializeField] FloatVariable _offsetX;
    [SerializeField] FloatVariable _offsetY;

    Nebula2 _nebula2;

    SpriteRenderer _spriteRenderer;
    MaterialPropertyBlock _materialBlock;
    Texture2D _texture;

    Color[] _textureColors;

    public void GetNoiseArray(ref NativeArray<float> outNoise)
    {
        Assert.AreEqual(outNoise.Length, Nebula2.noiseWidth * Nebula2.noiseHeight);
        _textureColors = _texture.GetPixels();

        CalcNoiseArray(
            textureWidth: _texture.width,
            textureHeight: _texture.height,
            noiseWidth: Nebula2.noiseWidth,
            noiseHeight: Nebula2.noiseHeight,
            scale: _scale.value,
            offsetX: _offsetX.value,
            offsetY: _offsetY.value,
            textureNoise: _textureColors,
            ref outNoise
        );
    }

    [BurstCompile]
    void CalcNoiseArray(
        int textureWidth,
        int textureHeight,
        int noiseWidth,
        int noiseHeight,
        float scale,
        float offsetX,
        float offsetY,
        Color[] textureNoise,
        ref NativeArray<float> outNoise
    )
    {
        float2 ratio = new float2(0, 0);
        ratio.x = (float)textureWidth / noiseWidth;
        ratio.y = (float)textureHeight / noiseHeight;
        int i = 0;
        for (int y = 0; y < noiseHeight; y++)
        {
            for (int x = 0; x < noiseWidth; x++)
            {
                int newX = (int)math.floor(x * ratio.x * scale + textureWidth * offsetX) % textureWidth;
                int newY = (int)math.floor(y * ratio.y * scale + textureHeight * offsetY) % textureHeight;
                i = newX + newY * textureWidth;
                outNoise[x + y * noiseWidth] = textureNoise[i].r * textureNoise[i].a;
            }
        }
    }

    void Awake()
    {
        _nebula2 = FindObjectOfType<Nebula2>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(_spriteRenderer);
        _materialBlock = new MaterialPropertyBlock();
        _texture = _spriteRenderer.sprite.texture;
        Assert.IsNotNull(_texture);
    }

    void Update()
    {
        _spriteRenderer.GetPropertyBlock(_materialBlock);
        _materialBlock.SetVector("_MainTex_ST", new Vector4(_scale.value, _scale.value, _offsetX.value, _offsetY.value));
        _spriteRenderer.SetPropertyBlock(_materialBlock);
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        if (_nebula2 == null) return;
        _nebula2.GenerateNoise();
        _nebula2.DrawOutput();
    }
}