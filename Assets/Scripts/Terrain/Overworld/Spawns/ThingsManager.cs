﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Spawn
{
    public Vector3 location;
    public Things thing;
}

public enum Things
{
    None,
    Tree,
    Rock,
    MetalNode,
    Crystal
}

public class ThingsManager : MonoBehaviour
{

}