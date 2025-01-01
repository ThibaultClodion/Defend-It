using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum CharacterStatus
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

[RequireComponent(typeof(Rigidbody2D), typeof(CustomAnimator))]
public class Character : MonoBehaviour
{
    //Static variables
    public static int MAX_ABILITIES = 4;

    [Header("Character data")]
    [SerializeField] public CharacterData data;
    [SerializeField] public bool isEnnemy = false;
    [NonSerialized] public bool dataUpdated = false;

    [Header("GameObjects")]
    [SerializeField] private GameObject spellGO;
    private GameObject parentSpellsGO;

    // Variables mouvement
    private ContactFilter2D movementFilter = new ContactFilter2D();
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private CustomAnimator customAnimator;
    //private float hitDodgeCooldown = 1.0f;
    //private float hitDodgeTime = 2.0f;
    private bool lookingAtRight = true;

    //Stats
    private int actualHealth = 0;
    private bool[] spellCooldown = new bool[MAX_ABILITIES];

    //Spells
    private SpellCasting spellCasting;
    //private List<CharacterStatus> status = new List<CharacterStatus>();

    private void Awake()
    {
        parentSpellsGO = GameObject.Find("SpellContainer");
        if(parentSpellsGO == null)
        {
            Debug.LogWarning("SpellContainer not found");
            parentSpellsGO = gameObject;
        }
        movementFilter.SetLayerMask(LayerMask.GetMask("MapCollide"));
        if (isEnnemy)
        {
            movementFilter.layerMask += LayerMask.GetMask("Construction");
        }
        movementFilter.useLayerMask = true;

        rb = GetComponent<Rigidbody2D>();
        customAnimator = GetComponent<CustomAnimator>();
        customAnimator.SetAnimations(data.animations);
        actualHealth = data.stats.health;
        spellCasting = gameObject.AddComponent<SpellCasting>();
        spellCasting.Initialize(this, parentSpellsGO, spellGO);

        //All the cooldown are refresh
        for (int i = 0; i < spellCooldown.Length; i++)
        {
            spellCooldown[i] = true;
        }
    }

    IEnumerator DestroyFast()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    public bool CanLaunchSpell(int index)
    {
        return spellCooldown[index];
    }

    private IEnumerator ResetSpellCooldownAfterCooldown(int index)
    {
        yield return new WaitForSecondsRealtime(data.spells[index].cooldown);
        spellCooldown[index] = true;
    }

    public Bounds PreLaunchSpell(int index)
    {
        SpellData spell = data.spells[index];
        if (spell == null || spell.getType() != SpellType.PROJECTILE)
            return new Bounds();

        return spell.spellSprite.bounds;
    }

    public void LaunchSpell(int index, Vector2 direction)
    {
        if (CanLaunchSpell(index))
        {
            customAnimator.PlayAnim(CustomAnimator.GetSpellAnimByIndex(index), 1, true);
            spellCasting.LaunchSpell(data.spells[index], direction);
            spellCooldown[index] = false;
            StartCoroutine(ResetSpellCooldownAfterCooldown(index));
        }
    }

    public void Move(Vector2 movementInput)
    {
        //Move if the character want to
        if (movementInput != Vector2.zero)
        {
            //Allow to 'slide' on obstacle instead of being blocked
            customAnimator.PlayAnim(CustomAnimator.CharacterAnimState.MOVE);
            if (TryMove(movementInput)) { }
            else if (TryMove(new Vector2(movementInput.x, 0))) { }
            else if (TryMove(new Vector2(0, movementInput.y))) { }

            // Rotate on himself when going to left or right side (memorize the position if don't move horizontally)
            if (movementInput.x < 0)
            {
                if (lookingAtRight)
                {
                    transform.Rotate(0, 180, 0);
                    lookingAtRight = false;
                }
            }
            else if(movementInput.x > 0)
            {
                if (!lookingAtRight)
                {
                    transform.Rotate(0, 180, 0);
                    lookingAtRight = true;
                }
            }
        }
        //If the character doesn't move
        else
        {
            customAnimator.PlayAnim(CustomAnimator.CharacterAnimState.IDLE);
        }
    }

    public bool TryMove(Vector2 direction)
    {
        float collisionOffset = 0.1f;
        int count = rb.Cast(direction, movementFilter, castCollisions, data.stats.moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * data.stats.moveSpeed * Time.fixedDeltaTime);
        }

        return (count == 0);
    }

    public void Hit(int damage)//, List<CharacterStatus> ApplyedStatut, float time)
    {
        //if (hitDodgeTime < hitDodgeCooldown)
        //    return;
        //hitDodgeTime = 0;
        customAnimator.PlayAnim(CustomAnimator.CharacterAnimState.HIT, 1, true);
        actualHealth -= damage;

        //Destroy the GameObject if the character has no health
        if(actualHealth < 0)
        {
            Destroy(this.gameObject);
        }

        return;
    }

    public void TransformInto(CharacterData characterData)
    {
        //ca va peter ?
        dataUpdated = true;
        data = characterData;
        customAnimator.SetAnimations(data.animations);
    }
}
