using UnityEngine;

public class GlowSpot : MonoBehaviour
{
    [SerializeField]
    private float glow_strength; // + 0.01 GTL.s

    private PlayerData pd;
    private bool player_in_spotlight = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pd = other.gameObject.GetComponentInParent<PlayerData>();
            player_in_spotlight = true;
        }
        else
            Debug.Log("not player");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player_in_spotlight = false;
    }

    private void FixedUpdate()
    {
        if (player_in_spotlight && pd != null)
            pd.AddGlow(glow_strength * Time.fixedDeltaTime * 0.01f);

    }
}
