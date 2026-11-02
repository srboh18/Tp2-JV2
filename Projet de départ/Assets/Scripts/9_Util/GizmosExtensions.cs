using UnityEngine;

public static class GizmosExtensions
{
    /// <summary>
    /// Draw arrow in the scene.
    /// </summary>
    /// <param name="from">Start position.</param>
    /// <param name="to">End position. This is where the arrow points.</param>
    public static void DrawArrow(Vector3 from, Vector3 to)
    {
        DrawArrow(from, to, Color.white);
    }

    /// <summary>
    /// Draw arrow in the scene.
    /// </summary>
    /// <param name="from">Start position.</param>
    /// <param name="to">End position. This is where the arrow points.</param>
    /// <param name="color">Color.</param>
    public static void DrawArrow(Vector3 from, Vector3 to, Color color)
    {
        DrawArrow(from, to, Color.white, 1);
    }
    
    /// <summary>
    /// Draw arrow in the scene.
    /// </summary>
    /// <param name="from">Start position.</param>
    /// <param name="to">End position. This is where the arrow points.</param>
    /// <param name="color">Color.</param>
    /// <param name="size">Arrow head size.</param>
    public static void DrawArrow(Vector3 from, Vector3 to, Color color, float size)
    {
        var colorBackup = Gizmos.color;
        Gizmos.color = color;

        var direction = Quaternion.LookRotation(to - from);
        var arrowSize = Vector3.forward * size;
        var up = direction * Quaternion.Euler(180 + 30, 0, 0) * arrowSize;
        var down = direction * Quaternion.Euler(180 - 30, 0, 0) * arrowSize;
        var left = direction * Quaternion.Euler(0, 180 + 30, 0) * arrowSize;
        var right = direction * Quaternion.Euler(0, 180 - 30, 0) * arrowSize;

        Gizmos.DrawLine(from, to);
        Gizmos.DrawLine(to, to + up);
        Gizmos.DrawLine(to, to + down);
        Gizmos.DrawLine(to, to + left);
        Gizmos.DrawLine(to, to + right);
        Gizmos.color = colorBackup;
    }
}