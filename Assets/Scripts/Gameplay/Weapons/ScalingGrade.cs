using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScalingGrade 
{
   S,
   A,
   B,
   C,
   D,
   E,
   None
}

public static class Scaling
{
    public static float GetScalingMultiplier(ScalingGrade grade)
    {
        switch (grade)
        {
            case ScalingGrade.S: return 1.0f;
            case ScalingGrade.A: return 0.75f;
            case ScalingGrade.B: return 0.6f;
            case ScalingGrade.C: return 0.45f;
            case ScalingGrade.D: return 0.3f;
            case ScalingGrade.E: return 0.15f;
            default: return 0f;
        }
    }
}