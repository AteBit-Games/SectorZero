/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.BehaviourTree;
using UnityEngine;

namespace Runtime.Utils
{
    public static class OperationUtils
    {
        public static bool Compare(float a, float b, CompareMethod cm, float floatingPoint) {
            return cm switch
            {
                CompareMethod.EqualTo => Mathf.Abs(a - b) <= floatingPoint,
                CompareMethod.GreaterThan => a > b,
                CompareMethod.LessThan => a < b,
                CompareMethod.GreaterOrEqualTo => a >= b,
                CompareMethod.LessOrEqualTo => a <= b,
                _ => true
            };
        }

        public static bool Compare(int a, int b, CompareMethod cm) {
            return cm switch
            {
                CompareMethod.EqualTo => a == b,
                CompareMethod.GreaterThan => a > b,
                CompareMethod.LessThan => a < b,
                CompareMethod.GreaterOrEqualTo => a >= b,
                CompareMethod.LessOrEqualTo => a <= b,
                _ => true
            };
        }
    }
}