using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class WorldToCell
{
    public static Vector3[] GetSpriteCorners(SpriteRenderer renderer)
    {
        Vector3 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector3 topLeft = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y, 0));
        Vector3 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector3 botRight = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y, 0));
        return new Vector3[] { topRight, topLeft, botLeft, botRight };
    }
}

