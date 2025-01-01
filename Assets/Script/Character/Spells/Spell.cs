using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public ContactFilter2D collisionFilter;

    //Data coming from Character
    public SpellProjectileData data = null;
    public Vector2 direction;
    private bool readyToLaunch = false;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool init = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        collisionFilter.SetLayerMask(LayerMask.GetMask("MapCollide"));
    }

    //Touch les ennemis une fois par seconde
    void FixedUpdate()
    {
        if (!readyToLaunch)
            return;

        if(init)
        {
            int hits = HitTouchedEnnemiesWithRB();
            
            if (!data.directToLocation)
            {
                rb.MovePosition(rb.position + direction * data.travelSpeed * Time.fixedDeltaTime);
                if (hits > 0 && !data.getThrought)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void Init()
    {
        if (spriteRenderer != null && data != null)
        {
            spriteRenderer.sprite = data.spellSprite;
            if(data.directToLocation)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }

            Destroy(this.gameObject, data.duration);
            init = true;
        }
        else
        {
            Debug.LogError("spriteRender or data not attached to Launched Spell");
        }
    }
    
    private int HitTouchedEnnemiesWithRB()
    {
        float collisionOffset = 0.05f;
        castCollisions.Clear();
        int count = rb.Cast(direction, collisionFilter, castCollisions, data.travelSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count > 0)
        {
            foreach (RaycastHit2D hit in castCollisions)
            {
                Character hitCharacter;
                if (hit.transform.TryGetComponent(out hitCharacter))
                {
                    hitCharacter.Hit(data.damage);
                    Debug.Log("hit");
                }
            }
        }
        return count;
    }

    public void Launch()
    {
        readyToLaunch = true;
        Init();
    }
}