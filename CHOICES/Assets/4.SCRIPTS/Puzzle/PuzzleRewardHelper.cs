#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PuzzleRewardHelper : MonoBehaviour
{
    public const string GlowSpotTag = "GlowSpotHolder";
    public PuzzleRewards helperTarget;

    public bool RefreshGlowSpots = false;

    void Start()
    {
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (RefreshGlowSpots){ FindAllGlowSpots(); }
    }

    public void FindAllGlowSpots()
    {
        // Glow spots for power plant
        helperTarget.LightsToActivate = new List<GlowSpot>();
        List<GameObject> spots = new List<GameObject>( GameObject.FindGameObjectsWithTag(GlowSpotTag) );
        foreach (GameObject o in spots)
        {
            GlowSpot spot = o.GetComponentInChildren<GlowSpot>(true);
            if (spot==null)
            {
                Debug.LogWarning("No GlowSpot under given GlowSpotHolder : " + o.name);
                continue;
            }
            helperTarget.LightsToActivate.Add(spot);
            spot.gameObject.SetActive(false);
        }
        RefreshGlowSpots = false; 
    }
}

#endif
