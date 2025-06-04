using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventLog;

public static class OTCShapeChecker
{
    public static float GetTriangleMatching(List<Vector3> iPositions)
    {
        float matchScore = 0f;

        int n_pos = iPositions.Count;
        if ((n_pos % 3) != 0)
        {
            // cannot be ordered triangle
            // > if its not a multiple of 3, then one edge has more "positions" than the others
            // thus its chaotic
            INFO("Triangle matching is 0 because there is " + n_pos + " positions");
            return matchScore;
        }
        // Discard Unity Y-axis
        Vector2[] projectedPositions = new Vector2[n_pos];
        for (int i = 0; i < n_pos; i++)
        { projectedPositions[i] = new Vector2(iPositions[i].x, iPositions[i].z); }

        // step between summits indexes
        int summitSteps = n_pos / 3;

        // summit indexes
        int A = 0;
        int B = summitSteps;
        int C = summitSteps * 2;

        Vector2 AB = projectedPositions[B] - projectedPositions[A];
        Vector2 AC = projectedPositions[C] - projectedPositions[A];
        float angle_A = Vector2.Angle(AB, AC);

        Vector2 BA = projectedPositions[A] - projectedPositions[B];
        Vector2 BC = projectedPositions[C] - projectedPositions[B];
        float angle_B = Vector2.Angle(BA, BC);

        Vector2 CA = projectedPositions[A] - projectedPositions[C];
        Vector2 CB = projectedPositions[B] - projectedPositions[C];
        float angle_C = Vector2.Angle(BA, BC);

        // sum of angles needs to be 180f
        float sumOfAngles = angle_A + angle_B + angle_C;
        INFO("Triangle Check : sumOfAngles is " + sumOfAngles);

        float angleDiff = Mathf.Abs(180f - sumOfAngles);
        if (angleDiff <= GameSettings.Instance.TriangleMatchingAngleEps)
        {
            // perfect match
            matchScore = 1f;
            INFO("Triangle Check : Perfect  matching");
        }
        else
        {
            // compute penalty
            float angleDivergencePenalty = angleDiff - GameSettings.Instance.TriangleMatchingAngleEps;
            angleDivergencePenalty *= GameSettings.Instance.TriangleAngleDivergencePenaltyFactor;
            INFO("Triangle Check : Angle Divergence Penalty is " + angleDivergencePenalty);
            matchScore = Mathf.Clamp01(matchScore - angleDivergencePenalty);
        }

        // if > 3, check alignements of inbetween vertices positions

        // number of 'positions" inbetween pivot vertices(triangle corners)
        //int edgeSteps = summitSteps - 1;

        INFO("Triangle Check : Final match score is " + matchScore);
        return matchScore;
    }
}
