using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]

public class Alien
{
    public int spawnTime;
    public AlienType alienType;
    public int Spawner;
    public bool RandowmSpawn;
    public bool IsSpawned;
}

public enum AlienType
{
    Alien_Basic,
    Alien_Advanced,
    Alien_Girl,
}