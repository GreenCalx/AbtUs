using UnityEngine;

public static class Constants
{
    public const string suffix_Instance = "(Instance)";
    // ----------------------------------------------------
    // SHADERS
    // global parm
    public const string shad_lerpParm = "_Lerp";
    public const string shad_chaosParm = "_Chaos";
    // base shader parms
    public const string shad_baseAlbedo = "_Albedo";
    public const string shad_baseNormal = "_NormalMap";
    public const string shad_baseHeight = "_HeightMap";
    public const string shad_baseMask = "_Mask";
    public const string shad_baseMinMetallicRemap = "_MinMetallicRemap";
    public const string shad_baseMaxMetallicRemap = "_MaxMetallicRemap";
    public const string shad_baseMinSmoothnessRemap = "_MinSmoothnessRemap";
    public const string shad_baseMaxSmoothnessRemap = "_MaxSmoothnessRemap";
    // mixed shader parms
    public const string shad_MixedAlbedo = "_Mix_Albedo";
    public const string shad_MixedNormal = "_Mix_NormalMap";
    public const string shad_MixedHeight = "_Mix_HeightMap";
    public const string shad_MixedMask = "_Mix_Mask";
    public const string shad_MixedMinMetallicRemap = "_Mix_MinMetallicRemap";
    public const string shad_MixedMaxMetallicRemap = "_Mix_MaxMetallicRemap";
    public const string shad_MixedMinSmoothnessRemap = "_Mix_MinSmoothnessRemap";
    public const string shad_MixedMaxSmoothnessRemap = "_Mix_MaxSmoothnessRemap";
    // ----------------------------------------------------
}
