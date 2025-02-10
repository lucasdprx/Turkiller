using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _building;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        _building.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        _building.SetActive(true);
    }
}
