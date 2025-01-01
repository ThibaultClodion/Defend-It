using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class IAController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
        target = GameObject.FindGameObjectWithTag("Heart");
    }

    void Update()
    {
        Vector2Int direction = GetDirection(transform.position, target.transform.position, 0.5f);
        if(direction.x != 0 || direction.y != 0)
        {
            character.Move(direction);
        }
        else
        {
            character.LaunchSpell(0, direction);
        }
    }

    private int GetDirection(float position, float targetPosition, float seuil)
    {
        if (position - seuil > targetPosition)
            return -1;
        else if (position + seuil < targetPosition)
            return 1;
        return 0;
    }

    private Vector2Int GetDirection(Vector2 position, Vector2 targetPosition, float seuil=5f)
    {
        return new Vector2Int(GetDirection(position.x, targetPosition.x, seuil),
            GetDirection(position.y, targetPosition.y, seuil));
    }
}
