using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats player = collision.GetComponent<PlayerStats>();

        if (player)
        {
            Debug.Log(player.ToString());
            UIManager.Instance.NextLevel(player);
        }
    }
}
