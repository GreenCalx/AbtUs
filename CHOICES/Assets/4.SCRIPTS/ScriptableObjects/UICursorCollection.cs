using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UICursorCollection : MonoBehaviour
{
    public List<UICursorSO> cursors;

    public Sprite GetImageFromAction(PLAYER_ACTIONS iAction)
    {
        UICursorSO[] eligibles = cursors.Where(e => e.relatedAction == iAction).ToArray();
        if (eligibles.Length > 1)
        {
            Debug.LogWarning("More than one cursor defined for action " + iAction.ToString());
        }
        else if (eligibles.Length <= 0)
        {
            Debug.LogWarning("No cursor defined for action " + iAction.ToString());
            return null;
        }
        return eligibles[0].image;
    }

    public Sprite GetAltImageFromAction(PLAYER_ACTIONS iAction)
    {
        UICursorSO[] eligibles = cursors.Where(e => e.relatedAction == iAction).ToArray();
        if (eligibles.Length > 1)
        {
            Debug.LogWarning("More than one cursor defined for action " + iAction.ToString());
        }
        else if (eligibles.Length <= 0)
        {
            Debug.LogWarning("No cursor defined for action " + iAction.ToString());
            return null;
        }
        return eligibles[0].alt_image;
    }

    public Sprite GetHResImageFromAction(PLAYER_ACTIONS iAction)
    {
        UICursorSO[] eligibles = cursors.Where(e => e.relatedAction == iAction).ToArray();
        if (eligibles.Length > 1)
        {
            Debug.LogWarning("More than one cursor defined for action " + iAction.ToString());
        }
        else if (eligibles.Length <= 0)
        {
            Debug.LogWarning("No cursor defined for action " + iAction.ToString());
            return null;
        }
        return eligibles[0].hres_image;
    }

    public Sprite GetHResAltImageFromAction(PLAYER_ACTIONS iAction)
    {
        UICursorSO[] eligibles = cursors.Where(e => e.relatedAction == iAction).ToArray();
        if (eligibles.Length > 1)
        {
            Debug.LogWarning("More than one cursor defined for action " + iAction.ToString());
        }
        else if (eligibles.Length <= 0)
        {
            Debug.LogWarning("No cursor defined for action " + iAction.ToString());
            return null;
        }
        return eligibles[0].hres_alt_image;
    }
}
