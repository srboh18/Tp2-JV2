using UnityEngine;

public static class Vector3Extensions
{
    public static int CompareTo(this Vector3 lhs, Vector3 rhs)
    {
        var comparison = lhs.x.CompareTo(rhs.x);
        if (comparison != 0) return comparison;

        comparison = lhs.y.CompareTo(rhs.y);
        if (comparison != 0) return comparison;
        
        comparison = lhs.z.CompareTo(rhs.z);
        if (comparison != 0) return comparison;
        
        return 0;
    }
}