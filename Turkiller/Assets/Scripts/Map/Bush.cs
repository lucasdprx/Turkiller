using UnityEngine;
using UnityEngine.InputSystem;

public class Bush : MonoBehaviour
{
    private SpriteRenderer _spriteBush;
    private Color _colorClearBush;
    private Color _colorFullBush;
    private float _alphaValueClearBush = 0.6f;

    private void Awake()
    {
        _spriteBush = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _colorClearBush = new Color(1, 1, 1, _alphaValueClearBush);
        _colorFullBush = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        _spriteBush.color = _colorClearBush;
        print ("in");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        _spriteBush.color = _colorFullBush;
        print("out");
    }
}
