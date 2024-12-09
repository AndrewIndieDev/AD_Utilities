using AndrewDowsett.Utilities;
using System;
using UnityEngine;

namespace AndrewDowsett.Utility
{
    public static class UnitTest
    {
        public static void Execute(Action action, float repeatingTime = 1f, int maxIterations = 10, int iteration = 1)
        {
            FunctionPeriodic.Create(() =>
            {
                float startTime = Time.realtimeSinceStartup;
                action();
                Debug.Log($"[{iteration} / {maxIterations}] {action.Method.Name}: {(Time.realtimeSinceStartup - startTime) * 1000}ms");
            }, () =>
            {
                iteration++;
                return iteration > maxIterations;
            }, repeatingTime);
        }
    }
}