using UnityEngine;
using static Constants;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MatDefSO", menuName = "Scriptable Objects/MatDefSO")]
public class MatDefSO : ScriptableObject
{
    public Texture2D albedo;
    public Texture2D normals;
    public Texture2D height;
    public Texture2D mask;

    public float minMetallicRemap;
    public float maxMetallicRemap;
    public float minSmoothnessRemap;
    public float maxSmoothnessRemap;

    public void init(Material iMat)
    {
        albedo  = (Texture2D) iMat.GetTexture(shad_baseAlbedo );
        normals = (Texture2D) iMat.GetTexture(shad_baseNormal );
        height  = (Texture2D) iMat.GetTexture(shad_baseHeight );
        mask    = (Texture2D) iMat.GetTexture(shad_baseMask );
        
        minMetallicRemap = iMat.GetFloat(shad_baseMinMetallicRemap);
        maxMetallicRemap = iMat.GetFloat(shad_baseMaxMetallicRemap);
        minSmoothnessRemap = iMat.GetFloat(shad_baseMinSmoothnessRemap);
        maxSmoothnessRemap = iMat.GetFloat(shad_baseMaxSmoothnessRemap);
    }

}
