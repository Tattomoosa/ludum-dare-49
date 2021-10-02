using UnityEngine;

public class KillZone : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        player.Die();
    }
}