using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public SpellProjectileData projectileData = null;
    [SerializeField] private GameObject spellGO;

    private void Start()
    {
        StartCoroutine(LaunchSpells());
    }

    IEnumerator LaunchSpells()
    {
        if(projectileData != null)
        {
            yield return new WaitForSeconds(projectileData.cooldown);
            LaunchSpell();
            StartCoroutine(LaunchSpells());
        }
    }

    public void LaunchSpell()
    {
        //Find the ennemi to define the direction
        Vector2 direction = new Vector2(1,1);

        Vector2 spawnPosition = transform.position;
        Quaternion spawnRotation = new Quaternion();

        if (projectileData.directToLocation)
        {
            spawnPosition = transform.position;
            float m_Angle = Vector2.Angle(Vector2.right, direction - spawnPosition);
            //Debug.DrawLine(Vector2.right, direction - spawnPosition, Color.white, 20f);
            float directiony = direction.y > spawnPosition.y ? 1 : -1;
            spawnRotation = Quaternion.AngleAxis(m_Angle * directiony, Vector3.forward);
            //spawnPosition += new Vector2(5, 0);
        }

        GameObject createdSpellGO = Instantiate(spellGO, spawnPosition, spawnRotation);
        createdSpellGO.transform.parent = transform;
        Spell launchedSpell = createdSpellGO.AddComponent<Spell>();
        launchedSpell.data = projectileData;
        launchedSpell.direction = direction - new Vector2(transform.position.x, transform.position.y);
        launchedSpell.Launch();
    }
}
