using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int health = 100;
    public float moveSpeed = 3f;
    public int difficultyScore;
}

[Serializable]
public class CharacterAnimations
{
    [Header("Move animations")]
    public List<Sprite> idleAnimations;
    public List<Sprite> moveAnimations;
    [Header("Spells animations")]
    public List<Sprite> spell1Animations;
    public List<Sprite> spell2Animations;
    public List<Sprite> spell3Animations;
    public List<Sprite> spell4Animations;
    [Header("Hit animations")]
    public List<Sprite> hitAnimations;
}

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public Stats stats;
    public SpellData[] spells = new SpellData[4];
    public CharacterAnimations animations;
}