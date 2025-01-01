using System;
using UnityEngine;

public class SpellCasting : MonoBehaviour 
{
    private GameObject parentSpellsGO;
    private GameObject spellGO;
    private Character character;
    private bool isEnnemy;

    public void Initialize(Character _character, GameObject _parentSpellsGO, GameObject _spellGO)
    {
        character = _character;
        spellGO = _spellGO;
        parentSpellsGO = _parentSpellsGO;
    }

    public void LaunchSpell(SpellData spell, Vector2 direction)
    {
        if (spell == null || parentSpellsGO == null || spellGO == null)
        {
            Debug.Log("Spell GO null");
            return;
        }

        if (spell.getType() == SpellType.PROJECTILE)
        {
            ProjectileSpell((SpellProjectileData)spell, direction);
        }
        else if(spell.getType() == SpellType.MELEE)
        {
            MeleeSpell((SpellMeleeData)spell, direction);
        }
        else if (spell.getType() == SpellType.TRANSFORMATION && character != null)
        {
            TransformSpell((SpellTransformationData)spell);
        }
        else if (spell.getType() == SpellType.INVOCATION)
        {

        }
    }

    private void TransformSpell(SpellTransformationData spell)
    {
        character.TransformInto(spell.character);
    }

    private void ProjectileSpell(SpellProjectileData spell, Vector2 direction)
    {
        Vector2 spawnPosition = direction;
        Quaternion spawnRotation = new Quaternion();

        if (!spell.directToLocation)
        {
            spawnPosition = transform.position;
            float m_Angle = Vector2.Angle(Vector2.right, direction - spawnPosition);
            //Debug.DrawLine(Vector2.right, direction - spawnPosition, Color.white, 20f);
            float directiony = direction.y > spawnPosition.y ? 1 : -1;
            spawnRotation = Quaternion.AngleAxis(m_Angle * directiony, Vector3.forward);
            //spawnPosition += new Vector2(5, 0);
        }

        GameObject createdSpellGO = Instantiate(spellGO, spawnPosition, spawnRotation);
        createdSpellGO.transform.parent = parentSpellsGO.transform;
        Spell launchedSpell = createdSpellGO.AddComponent<Spell>();
        launchedSpell.data = spell;
        launchedSpell.direction = direction - new Vector2(transform.position.x, transform.position.y);

        if(character.isEnnemy)
        {
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Construction");
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Player");
        }
        else
        {
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Ennemy");
        }
        launchedSpell.collisionFilter.useLayerMask = true;
        launchedSpell.Launch();
    }

    private void MeleeSpell(SpellMeleeData spell, Vector2 direction)
    {
        Vector2 spawnPosition = direction;
        Quaternion spawnRotation = new Quaternion();

      
        spawnPosition = transform.position;
        float m_Angle = Vector2.Angle(Vector2.right, direction - spawnPosition);
        float directiony = direction.y > spawnPosition.y ? 1 : -1;
        spawnRotation = Quaternion.AngleAxis(m_Angle * directiony, Vector3.forward);
       
        GameObject createdSpellGO = Instantiate(spellGO, spawnPosition, spawnRotation);
        createdSpellGO.transform.parent = parentSpellsGO.transform;
        SpellMelee launchedSpell = createdSpellGO.AddComponent<SpellMelee>();
        launchedSpell.data = spell;
        launchedSpell.direction = direction - new Vector2(transform.position.x, transform.position.y);

        if (character.isEnnemy)
        {
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Construction");
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Player");
        }
        else
        {
            launchedSpell.collisionFilter.layerMask += LayerMask.GetMask("Ennemy");
        }
        launchedSpell.collisionFilter.useLayerMask = true;
        launchedSpell.Launch();
    }
}
