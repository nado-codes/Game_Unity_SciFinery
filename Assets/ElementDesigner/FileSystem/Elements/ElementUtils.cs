using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ElementUtils
{
    public static bool CanFuse(WorldElement a, WorldElement b)
    {
        var isNucleic = a.Charge >= 0 && b.Charge >= 0;
        var distance = Vector3.Distance(a.transform.position, b.transform.position);
        return isNucleic && distance < 2;
    }

    public static Vector3 ForceBetween(this WorldElement element, WorldElement otherElement)
    {
        var effectiveCharge = otherElement.Charge * element.Charge;

        var xBody = otherElement.transform.Find("Body");
        var body = element.transform.Find("Body");
        var massOffset = 1 / (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * element.MassMultiplier;

        var distanceToParticle = Vector3.Distance(xBody.transform.position, element.transform.position);
        var distanceScalar = 1 / (distanceToParticle > 0 ? distanceToParticle : 1);

        var dir = element.transform.position - otherElement.transform.position;
        var useNuclear = CanFuse(element, otherElement);

        if (useNuclear) Debug.DrawLine(element.transform.position, otherElement.transform.position, Color.red, 5000);

        effectiveCharge = useNuclear ? effectiveCharge * -1 : effectiveCharge;

        return dir * effectiveCharge * massOffset * distanceScalar;
    }

    public static string GetComposition(this IEnumerable<WorldElement> elements)
        => GetComposition(elements.Select(e => e.Data));

    public static string GetComposition(this Element element)
        => GetComposition(element.Children);

    public static string GetComposition(this IEnumerable<Element> elements, bool isUI = false)
    {
        var uniqueChildren = elements.GroupBy(ch => ch.Id).Select(ch => ch.First());
        var composition = uniqueChildren.Select(ch =>
        {
            var count = elements.Count(other => other.Id == ch.Id);
            (string start, string end) supText = ((isUI ? "<sup>" : ""), (isUI ? "</sup>" : ""));

            return ch.ShortName + (count > 1 ? supText.start + count + supText.end : "");
        });

        return string.Join("", composition);
    }
}