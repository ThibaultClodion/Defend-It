using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Deck : ScriptableObject
{
    public List<Construct> cards;
}
