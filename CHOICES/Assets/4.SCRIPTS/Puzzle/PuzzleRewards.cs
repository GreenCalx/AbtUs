using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class PuzzleRewards : MonoBehaviour
{

    [Header("Power Plant Reward Refs ( use helper to fill )")]
    public List<GlowSpot> LightsToActivate;

    public void TurnOnGlowSpots()
    {
        foreach (GlowSpot gs in LightsToActivate)
        {
            gs.gameObject.SetActive(true);
        }
    }
}
