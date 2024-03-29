using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Utilities
{
    public static Vector3 Average(this IEnumerable<Vector3> vectors)
    {
        var length = vectors.Count();
        if (length == 0)
            return Vector3.zero;

        var aggregate = vectors.Aggregate((a, c) => new Vector3(a.x + c.x, a.y + c.y, a.z + c.z));
        return new Vector3(aggregate.x / length, aggregate.y / length, aggregate.z / length);
    }

    public static Element Copy(this Element element)
    {
        var elementJson = JsonUtility.ToJson(element);
        return copy(elementJson, element.ElementType);
    }

    private static Element copy(string json, ElementType type) =>
    type switch
    {
        ElementType.Particle => JsonUtility.FromJson<Particle>(json),
        ElementType.Atom => JsonUtility.FromJson<Atom>(json),
        ElementType.Molecule => JsonUtility.FromJson<Molecule>(json),
        ElementType.Product => JsonUtility.FromJson<Product>(json),
        _ => throw new NotImplementedException($"Element of type ${type} is not yet implemented in call to Copy")
    };

    public static Color BlendColors(Color[] colors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in colors)
            result += c;

        return result / colors.Length;
    }

    public static Color Emphasise(this Color color)
    {
        var rgb = new float[3] { color.r, color.g, color.b };
        var emphasised = rgb.Select(n => n == rgb.Max() ? n : 0).ToArray();
        var result = color;
        result.r = emphasised[0];
        result.g = emphasised[1];
        result.b = emphasised[2];

        return result;
    }
}