using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum SpellType
{
    NONE,
    PROJECTILE,
    INVOCATION,
    MELEE,
    DASH,
    TRANSFORMATION
}

public enum SpellEffect
{
    NONE,
    PULL,
    PUSH,
    SLOW,
    INVINCIBLE, // Ne perd plus de vie
    WEAKENED, // Ne peut plus attaquer
    MARK,
    RESET_MARK,
    BURN
}

public enum SpellTarget
{
    NONE,
    EMPTY,
    TOWER,
    WALL,
    CHARACTER
}

public enum LinkedSpellTrigger
{
    NONE,
    SPELL_1,
    SPELL_2,
    SPELL_3,
    SPELL_4,
    TIME_AVAILABLE_END
}

/*
 * Lancer en plusieurs fois
 * Portail Fixe 
 * Zone d'effet ?
 * 
 */

[Serializable]
public class LinkedSpell
{
    public SpellData spellData;
    // Temps disponible 
    public int timeAvailable;
    public LinkedSpellTrigger trigger;
}

public class SpellData : ScriptableObject
{
    [Header("General")]
    public Sprite logoSprite;
    public Sprite spellSprite;
    public SpellTarget target;
    public float cooldown;
    public bool cantMove;

    [Header("Munitions (si > 1)")]
    public int munition;
    public float reloadCooldown;
    public bool reloadAll;
    public SpellEffect ReloadEffect;

    [Header("LinkedSpell")]
    public List<LinkedSpell> linkedSpells;
    protected int currentLinkedSpell;

    protected SpellType type;
    public virtual SpellType getType() { return SpellType.NONE; }
}

#region Types de Sorts 

public enum SpellFormation
{
    LINE,
    WAVE
}

//Faire la prévisualtion des sorts projectile
[CreateAssetMenu]
public class SpellProjectileData : SpellData
{
    [Header("Projectile Specific")]
    public int damage;
    public int duration;
    public bool directToLocation;
    public float travelSpeed;
    public bool getThrought;
    public int getTroughtUntil;
    public List<SpellEffect> effects;

    [Header("Multiple Projectiles")]
    public int numberOfProjectiles = 1;
    public SpellFormation spellFormation; // Remplacer par une list de vector2 ? -> Pour avoir une trajectoire précise par projectile

    public override SpellType getType()
    {
        return SpellType.PROJECTILE;
    }
}

[CreateAssetMenu]
public class SpellInvocationData : SpellData
{
    [Header("Invocation Specific")]
    public IAController controller;
    public Construct construct;
    public Character character;
    public CustomAnimator animator;
    public int number;
    public int duration;

    public override SpellType getType()
    {
        return SpellType.INVOCATION;
    }
}

[CreateAssetMenu]
public class SpellMeleeData : SpellData
{
    [Header("Melee Specific")]
    public int damage;

    public override SpellType getType()
    {
        return SpellType.MELEE;
    }
}

[CreateAssetMenu]
public class SpellTransformationData : SpellData
{
    [Header("Transformation Specific")]
    public CharacterData character;
    public int duration;

    public override SpellType getType()
    {
        return SpellType.TRANSFORMATION;
    }
}
#endregion