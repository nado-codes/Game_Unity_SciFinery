using UnityEngine;
public static class WorldUtilities
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
        effectiveCharge = useNuclear ? effectiveCharge * -1 : effectiveCharge;

        return dir * effectiveCharge * massOffset * distanceScalar;
    }
}