using UnityEngine;

public static class Utilities
{
    public static bool IsInsideRange(Vector3 positionToCheck, Vector3 originPosition, float range)
    {
        return (range >= (Vector3.Distance(positionToCheck, originPosition)));
    }

    public static bool IsInsideRange(GameObject positionToCheck, GameObject originPosition, float range)
    {
        return (range >= (Vector3.Distance(positionToCheck.transform.position, originPosition.transform.position)));
    }

    public static bool IsInAngle(Vector3 positionToCheck, Vector3 originPosition, float coneAngle)
    {
        float angleToTarget = Vector3.Angle(positionToCheck, originPosition);
        return (angleToTarget <= coneAngle / 2.0f);
    }

    // Nota: No reuso las funciones de IsInRange ni IsInAngle porque ser�a recalcular un par de cosas.
    public static bool IsInCone(Vector3 positionToCheck, Vector3 originPosition, float range, float coneAngle)
    {
        float angleToTarget = Vector3.Angle(positionToCheck, originPosition);
        // Si s� est� en rango y adem�s est� en el �ngulo de la visi�n,
        // entonces est� en el cono.
        return (range >= (Vector3.Distance(positionToCheck, originPosition)))
            && (angleToTarget <= coneAngle / 2.0f);
    }

    public static bool IsInCone(GameObject positionToCheck, GameObject originPosition, float range, float coneAngle)
    {
        return IsInCone(positionToCheck.transform.position, originPosition.transform.position, range, coneAngle);
    }
}
