using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class Utilities
{
    public static Vector3 ForceBetween(this WorldElement element, WorldElement otherElement)
    {
        var effectiveCharge = otherElement.Charge * element.Charge;
        var xBody = otherElement.transform.Find("Body");
        var body = element.transform.Find("Body");
        var massOffset = 1 / (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * element.MassMultiplier;

        var distanceToParticle = Vector3.Distance(xBody.transform.position, element.transform.position);
        var distanceOffset = 1 / (distanceToParticle > 0 ? distanceToParticle : 1);

        // .. comment this out to enable repulsive forces
        //effectiveCharge = effectiveCharge == 1 ? -1 : effectiveCharge;

        var dirTo = element.transform.position - otherElement.transform.position;

        return dirTo * effectiveCharge * massOffset * distanceOffset;
    }

    public static Vector3 Average(this IEnumerable<Vector3> vectors)
    {
        var length = vectors.Count();
        if (length == 0)
            return Vector3.zero;

        var aggregate = vectors.Aggregate((a, c) => new Vector3(a.x + c.x, a.y + c.y, a.z + c.z));
        return new Vector3(aggregate.x / length, aggregate.y / length, aggregate.z / length);
    }
}