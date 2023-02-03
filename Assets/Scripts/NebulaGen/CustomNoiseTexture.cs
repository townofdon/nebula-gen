using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

using NebulaGen;
using CyberneticStudios.SOFramework;
using System;

public class CustomNoiseTexture : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] Texture2DVariable _texture;
    [SerializeField] FloatVariable _scale;
    [SerializeField] FloatVariable _offsetX;
    [SerializeField] FloatVariable _offsetY;

    Nebula2 _nebula2;

    SpriteRenderer _spriteRenderer;
    MaterialPropertyBlock _materialBlock;

    Color[] _textureColors;

    public void GetNoiseArray(ref NativeArray<float> outNoise, float mixAmount)
    {
        Assert.AreEqual(outNoise.Length, Nebula2.noiseWidth * Nebula2.noiseHeight);
        _textureColors = _texture.value.GetPixels();

        CalcNoiseArray(
            textureWidth: _texture.value.width,
            textureHeight: _texture.value.height,
            noiseWidth: Nebula2.noiseWidth,
            noiseHeight: Nebula2.noiseHeight,
            scale: _scale.value,
            offsetX: _offsetX.value,
            offsetY: _offsetY.value,
            textureNoise: _textureColors,
            mixAmount: mixAmount,
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
        float mixAmount,
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
                outNoise[x + y * noiseWidth] = textureNoise[i].r * textureNoise[i].a * mixAmount;
            }
        }
    }

    void Awake()
    {
        _nebula2 = FindObjectOfType<Nebula2>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(_spriteRenderer);
        Assert.IsNotNull(_texture);
        _materialBlock = new MaterialPropertyBlock();
        _texture.ResetVariable();
        // _texture = _spriteRenderer.sprite.texture;
        Assert.IsNotNull(_texture);
        _texture.OnChanged += OnTextureChanged;
    }

    void OnDestroy()
    {
        _texture.OnChanged -= OnTextureChanged;
    }

    void Update()
    {
        _spriteRenderer.GetPropertyBlock(_materialBlock);
        _materialBlock.SetVector("_MainTex_ST", new Vector4(_scale.value, _scale.value, _offsetX.value, _offsetY.value));
        _spriteRenderer.SetPropertyBlock(_materialBlock);
    }

    void OnTextureChanged(Texture2D incoming)
    {
        _nebula2.GenerateNoise();
        _nebula2.DrawOutput();
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        if (_nebula2 == null) return;
        _nebula2.GenerateNoise();
        _nebula2.DrawOutput();
    }
}