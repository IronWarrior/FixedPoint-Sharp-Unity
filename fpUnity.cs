using UnityEngine;

namespace Deterministic.FixedPoint
{
    public static class fpUnity
    {
        public static Vector2 AsVector2(this fp2 fp) => new Vector2(fp.x.AsFloat, fp.y.AsFloat);
        public static Vector3 AsVector3(this fp3 fp) => new Vector3(fp.x.AsFloat, fp.y.AsFloat, fp.z.AsFloat);
    }
}
