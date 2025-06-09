using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventLog;

public static class OTCShapeChecker
{

    public static float GetShapeMatching(List<Vector3> iPositions)
    {
        int n_pos = iPositions.Count;
        if ((n_pos % 3) == 0)
        { return GetTriangleMatching(iPositions); }
        else if ((n_pos % 4) == 0)
        { return GetSquareMatching(iPositions); }

        return 0f;
    }

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
        Project2D(iPositions, out projectedPositions);
        // for (int i = 0; i < n_pos; i++)
        // { projectedPositions[i] = new Vector2(iPositions[i].x, iPositions[i].z); }

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
        float angle_C = Vector2.Angle(CA, CB);

        // each angle must be as close to 180f/3f as possible
        // it implies same magnitude and thus isocele
        // maybe check for rect triangles also
        float angleTarget = 180f / 3f;
        float angle_A_diff = Mathf.Abs(angleTarget - angle_A);
        float angle_B_diff = Mathf.Abs(angleTarget - angle_B);
        float angle_C_diff = Mathf.Abs(angleTarget - angle_C);

        float sumOfAngles = angle_A + angle_B + angle_C;
        float totAngleDiff = angle_A_diff + angle_B_diff + angle_C_diff;
        INFO("Triangle Check : totAngleDiff is " + sumOfAngles);
        if (totAngleDiff <= GameSettings.Instance.TriangleMatchingAngleEps)
        {
            // perfect match
            matchScore = 1f;
            INFO("Triangle Check : Perfect  matching");
        }
        else
        {
            // compute penalty
            float angleDivergencePenalty = totAngleDiff - GameSettings.Instance.TriangleMatchingAngleEps;
            angleDivergencePenalty *= GameSettings.Instance.TriangleAngleDivergencePenaltyFactor;
            INFO("Triangle Check : Angle Divergence Penalty is " + angleDivergencePenalty + " ,matchScore= " + matchScore);
            matchScore = Mathf.Clamp01(matchScore - angleDivergencePenalty);
        }

        // if > 3, check alignements of inbetween vertices positions
        if (n_pos > 3)
        {
            // // number of 'positions" inbetween pivot vertices(triangle corners)
            // float misalignementPenalty = 0f;
            // int edgeSteps = summitSteps - 1;

            // for (int e = 0; e < 3; e++)
            // {
            //     int from = e * summitSteps;
            //     int to = (e + 1) * summitSteps;
            //     if (to > n_pos) { to = 0; }
            //     Vector2 alignTo = projectedPositions[to] - projectedPositions[from];

            //     for (int j = 0; j < edgeSteps; j++)
            //     {
            //         Vector2 toAlign = projectedPositions[from + j] - projectedPositions[from];
            //         float dp = Vector2.Dot(alignTo, toAlign);
            //         if (dp < GameSettings.Instance.AlignementCheckDotProdThreshold)
            //         {
            //             misalignementPenalty += (dp < 0f ? 1f : 1f - dp) * GameSettings.Instance.MisalignementPenaltyFactor;
            //         }
            //     }
            //     if (misalignementPenalty >= 1f)
            //         break;
            // }
            // INFO("Triangle Check : Alignement penalty : " + misalignementPenalty);
            float misalignementPenalty = GetAlignementPenalty(projectedPositions, summitSteps);
            matchScore = Mathf.Clamp01(matchScore - misalignementPenalty);
        }

        INFO("Triangle Check Done. matchScore= " + matchScore);
        return matchScore;
    }

    public static float GetSquareMatching(List<Vector3> iPositions)
    {
        float matchScore = 0f;

        int n_pos = iPositions.Count;
        if ((n_pos % 4) != 0)
        {
            // cannot be ordered triangle
            // > if its not a multiple of 3, then one edge has more "positions" than the others
            // thus its chaotic
            INFO("Square matching is 0 because there is " + n_pos + " positions");
            return matchScore;
        }
        // Discard Unity Y-axis
        Vector2[] projectedPositions = new Vector2[n_pos];
        Project2D(iPositions, out projectedPositions);

        int summitSteps = n_pos / 4;

        // summit indexes
        int A = 0;
        int B = summitSteps;
        int C = summitSteps * 2;
        int D = summitSteps * 3;

        Vector2 AB = projectedPositions[B] - projectedPositions[A];
        Vector2 AC = projectedPositions[C] - projectedPositions[A];
        float angle_A = Vector2.Angle(AB, AC);

        Vector2 BA = projectedPositions[A] - projectedPositions[B];
        Vector2 BC = projectedPositions[C] - projectedPositions[B];
        float angle_B = Vector2.Angle(BA, BC);

        Vector2 CD = projectedPositions[D] - projectedPositions[C];
        Vector2 CB = projectedPositions[B] - projectedPositions[C];
        float angle_C = Vector2.Angle(CD, CB);

        Vector2 DA = projectedPositions[A] - projectedPositions[D];
        Vector2 DC = projectedPositions[C] - projectedPositions[D];
        float angle_D = Vector2.Angle(DA, DC);

        // Each Angle must be close to 90d
        float angle_A_diff = Mathf.Abs(90f - angle_A);
        float angle_B_diff = Mathf.Abs(90f - angle_B);
        float angle_C_diff = Mathf.Abs(90f - angle_C);
        float angle_D_diff = Mathf.Abs(90f - angle_D);

        float totAngleDiff = angle_A_diff + angle_B_diff + angle_C_diff + angle_D_diff;
        INFO("Square Check : totAngleDiff is " + totAngleDiff);
        if (totAngleDiff <= GameSettings.Instance.SquareMatchingAngleEps)
        {
            // perfect match
            matchScore = 1f;
            INFO("Square Check : Perfect  matching");
        }
        else
        {
            // compute penalty
            float angleDivergencePenalty = totAngleDiff - GameSettings.Instance.SquareMatchingAngleEps;
            angleDivergencePenalty *= GameSettings.Instance.SquareAngleDivergencePenaltyFactor;
            INFO("Square Check : Angle Divergence Penalty is " + angleDivergencePenalty + " ,matchScore= " + matchScore);
            matchScore = Mathf.Clamp01(matchScore - angleDivergencePenalty);
        }

        if (n_pos > 4)
        {
            float misalignementPenalty = GetAlignementPenalty(projectedPositions, summitSteps);
            matchScore = Mathf.Clamp01(matchScore - misalignementPenalty);
        }

        INFO("Square Check Done. matchScore= " + matchScore);
        return matchScore;
    }


    public static float GetCircleMatching(List<Vector3> iPositions)
    {
        float matchScore = 0f;

        int n_pos = iPositions.Count;
        if (n_pos < 5)
        {
            // cannot be ordered triangle
            // > if its not a multiple of 3, then one edge has more "positions" than the others
            // thus its chaotic
            INFO("Circle matching is 0 because there is " + n_pos + " positions");
            return matchScore;
        }

        // Discard Unity Y-axis
        Vector2[] projectedPositions = new Vector2[n_pos];
        Project2D(iPositions, out projectedPositions);

        // find center position
        // average of sum of positions
        Vector2 center = Vector2.zero;
        foreach (Vector2 p in projectedPositions) { center += p; }
        center /= n_pos;
        Debug.DrawRay(new Vector3(center.x, 0, center.y), Vector3.up * 100f, Color.red, 5f);
        INFO("Circle matchin : center pos : " + center);

        // get mean rho by averging sums of distance to center
        float meanRho = 0f;
        foreach (Vector2 p in projectedPositions)
        { meanRho += Vector2.Distance(p, center); }
        meanRho /= n_pos;
        INFO("Circle matchin : mean rho : " + meanRho);

        // deduce theta and thus compute ideal XZ position 
        


        return matchScore;
    }

    private static float GetAlignementPenalty(Vector2[] i2DPositions, int iSummitSteps)
    {
        int n_pos = i2DPositions.Length;
        float misalignementPenalty = 0f;
        int edgeSteps = iSummitSteps - 1;
        for (int e = 0; e < 3; e++)
        {
            int from = e * iSummitSteps;
            int to = (e + 1) * iSummitSteps;
            if (to > n_pos) { to = 0; }
            Vector2 alignTo = i2DPositions[to] - i2DPositions[from];
            for (int j = 0; j < edgeSteps; j++)
            {
                Vector2 toAlign = i2DPositions[from + j] - i2DPositions[from];
                float dp = Vector2.Dot(alignTo, toAlign);
                if (dp < GameSettings.Instance.AlignementCheckDotProdThreshold)
                {
                    misalignementPenalty += (dp < 0f ? 1f : 1f - dp) * GameSettings.Instance.MisalignementPenaltyFactor;
                }
            }
            if (misalignementPenalty >= 1f)
                break;
        }
        INFO("Computed Alignement penalty : " + misalignementPenalty);
        return misalignementPenalty;
    }

    private static void Project2D(List<Vector3> iPositions, out Vector2[] ioProjected)
    {
        int n_pos = iPositions.Count;
        ioProjected = new Vector2[n_pos];
        for (int i = 0; i < n_pos; i++)
        { ioProjected[i] = new Vector2(iPositions[i].x, iPositions[i].z); }
    }

}
