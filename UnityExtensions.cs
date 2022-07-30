using UnityEngine;

namespace KikyoMod;

internal static class UnityExtensions
{
    private const float MaxAllowedValueTop = 3.402823E+7f;

    private const float MaxAllowedValueBottom = -3.402823E+7f;
    /*
     * original source code: https://github.com/RequiDev/ReMod.Core/blob/master/Unity/UnityExtensions.cs
     */

    public static string GetPath(this Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return current.parent.GetPath() + "/" + current.name;
    }

    public static bool IsSafe(this float f)
    {
        return !IsAbsurd(f);
    }

    public static bool IsSafe(this Vector3 v3)
    {
        return !IsAbsurd(v3) || !IsBad(v3);
    }

    public static bool IsSafe(this Quaternion q)
    {
        return !IsAbsurd(q) || !IsBad(q);
    }

    private static bool IsAbsurd(this float f)
    {
        return !(f > MaxAllowedValueBottom && f < MaxAllowedValueTop);
    }

    private static bool IsBad(this Vector3 v3)
    {
        return float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z) ||
               float.IsInfinity(v3.x) || float.IsInfinity(v3.y) || float.IsInfinity(v3.z);
    }

    private static bool IsBad(this Quaternion q)
    {
        return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w) ||
               float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
    }

    private static bool IsAbsurd(this Quaternion q)
    {
        return !(q.x > MaxAllowedValueBottom && q.x < MaxAllowedValueTop) ||
               !(q.y > MaxAllowedValueBottom && q.y < MaxAllowedValueTop) ||
               !(q.z > MaxAllowedValueBottom && q.z < MaxAllowedValueTop) ||
               (q.w > MaxAllowedValueBottom && q.w < MaxAllowedValueBottom);
    }

    private static bool IsAbsurd(this Vector3 v3)
    {
        return !(v3.x > MaxAllowedValueBottom && v3.x < MaxAllowedValueTop) ||
               !(v3.y > MaxAllowedValueBottom && v3.y < MaxAllowedValueTop) ||
               !(v3.z > MaxAllowedValueBottom && v3.z < MaxAllowedValueTop);
    }

    public static void Clamp(this Vector3 v3)
    {
        v3.x = Mathf.Clamp(v3.x, -512000f, 512000f);
        v3.y = Mathf.Clamp(v3.y, -512000f, 512000f);
        v3.z = Mathf.Clamp(v3.z, -512000f, 512000f);
    }

    public static void Clamp(this Quaternion v3)
    {
        v3.x = Mathf.Clamp(v3.x, -512000f, 512000f);
        v3.y = Mathf.Clamp(v3.y, -512000f, 512000f);
        v3.z = Mathf.Clamp(v3.z, -512000f, 512000f);
        v3.w = Mathf.Clamp(v3.w, -512000f, 512000f);
    }

    public static string ToCleanString(this Vector3 v3, string format = "F4")
    {
        return v3.ToString(format).Replace(" ", string.Empty).Trim('(', ')');
    }
}