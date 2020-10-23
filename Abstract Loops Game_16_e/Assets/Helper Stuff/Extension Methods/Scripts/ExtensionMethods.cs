using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Vector
public static partial class ExtensionMethods 
{
    public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3
            (
                (x != null) ? (float)x : v.x,
                (y != null) ? (float)y : v.y,
                (z != null) ? (float)z : v.z
            );
    }
}

//Transform
public static partial class ExtensionMethods
{
    public static void ChangePosition(this Transform t, float? x = null, float? y = null, float? z = null)
    {
        t.position = new Vector3
            (
                (x != null) ? (float)x : t.position.x,
                (y != null) ? (float)y : t.position.y,
                (z != null) ? (float)z : t.position.z
            );
    }   

    public static void ChangeEulerAngles(this Transform t, float? x = null, float? y = null, float? z = null)
    {
        t.eulerAngles = new Vector3
            (
                (x != null) ? (float)x : t.eulerAngles.x,
                (y != null) ? (float)y : t.eulerAngles.y,
                (z != null) ? (float)z : t.eulerAngles.z
            );
    }
}

//Color
public static partial class ExtensionMethods
{
    public static Color With(this Color c, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(r??c.r, g??c.g, b??c.b, a??c.a);
    }
}

//Vector2
public static partial class ExtensionMethods
{
    public static Vector2 With(this Vector2 v, float? x = null, float? y = null) => new Vector2(x??v.x, y??v.y);
}
