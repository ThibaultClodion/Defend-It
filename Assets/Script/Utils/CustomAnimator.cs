using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Séparer cette classe en deux
[RequireComponent(typeof(SpriteRenderer))] 
public class CustomAnimator : MonoBehaviour
{   
    public enum CharacterAnimState
    {
        NONE,
        IDLE,
        MOVE,
        SPELL1,
        SPELL2,
        SPELL3,
        SPELL4,
        HIT
    }
    private CharacterAnimState currentAnimState = CharacterAnimState.NONE;

    [SerializeField] private int framerate = 10;
    private SpriteRenderer spriteRenderer;
    private CharacterAnimations animations = null;
    List<Sprite> animsToUse;
    private int count;
    private float deltaStartAnim;
    private int actualIteration = 0;
    private int maxIteration = 0;
    public bool Locked { private set; get; }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnim(CharacterAnimState.IDLE);
        Locked = false;
    }

    void Update()
    {
        if(animations == null)
        {
            return;
        }

        float timeForChangeSprite = 1.0f / (float)framerate; // Dans le update pour pouvoir tester en direct avec un autre framerate -> Enlever plus tard
        deltaStartAnim += Time.deltaTime;
        if (deltaStartAnim > timeForChangeSprite * (count + 1))
        {
            if (count + 1 >= animsToUse.Count)
            {
                count = 0;
                deltaStartAnim = 0;
                actualIteration++;

                if(actualIteration == maxIteration)
                {
                    Locked = false;
                    PlayAnim(CharacterAnimState.IDLE);
                }
            }
            else
                count++;

            if (count >= animsToUse.Count)
            {
                //IA n'ont pas d'animation donc pour éviter qu'il nous crie dessus
                //Debug.LogError("Animation " + count.ToString() + " non renseignée pour " + currentAnimState.ToString());
                return;
            }
            spriteRenderer.sprite = animsToUse[count];
        }
    }

    // Si iteration == 0, alors joue l'anim en boucle 
    public void PlayAnim(CharacterAnimState animationToPlay, int iteration=0, bool playedUntilEnd=false)
    {
        if(iteration == 0 && playedUntilEnd)
        {
            Debug.LogError("Can't play Animation until end if iteration == 0");
            return;
        }

        if(Locked)
        {
            return;
        }

        if (animationToPlay != currentAnimState)
        {
            currentAnimState = animationToPlay;
            animsToUse = GetActualAnims();
            count = 0;
            deltaStartAnim = 0;
            actualIteration = 0;
            maxIteration = iteration;
            if (playedUntilEnd)
                Locked = true;
        }
    }

    public static CharacterAnimState GetSpellAnimByIndex(int index)
    {
        switch(index)
        {
            case 0: return CharacterAnimState.SPELL1;
            case 1: return CharacterAnimState.SPELL2;
            case 2: return CharacterAnimState.SPELL3;
            case 3: return CharacterAnimState.SPELL4;
        }
        return CharacterAnimState.IDLE;
    }

    public void SetAnimations(CharacterAnimations characterAnimations)
    {
        animations = characterAnimations;
        PlayAnim(CharacterAnimState.IDLE);
    }

    private ref List<Sprite> GetActualAnims()
    {
        switch (currentAnimState)
        {
            case CharacterAnimState.IDLE:
                return ref animations.idleAnimations;
            case CharacterAnimState.MOVE:
                return ref animations.moveAnimations;
            case CharacterAnimState.HIT:
                return ref animations.hitAnimations;
            case CharacterAnimState.SPELL1:
                return ref animations.spell1Animations;
            case CharacterAnimState.SPELL2:
                return ref animations.spell2Animations;
            case CharacterAnimState.SPELL3:
                return ref animations.spell3Animations;
            case CharacterAnimState.SPELL4:
                return ref animations.spell4Animations;
        }
        return ref animations.idleAnimations;
    }
}