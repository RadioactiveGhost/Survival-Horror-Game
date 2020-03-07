using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BiomeStats
{
    public float Amp;
    public float Freq;
    public float MaxY;
    public float MinY;
    public Biome biome;
    public HeightColor[] color;
}

[System.Serializable]
public struct HeightColor
{
    public string name;
    public Color color;
    public float height;
}

public enum Biome
{
    Plains,
    Forest,
    Mountain
}

public class Biomes : MonoBehaviour
{
    public List<BiomeStats> inspectorBiomes;
    public static List<BiomeStats> biomes;

    void Awake()
    {
        biomes = new List<BiomeStats>();
        biomes = inspectorBiomes;
    }
}