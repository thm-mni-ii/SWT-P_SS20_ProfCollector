using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a card mesh in the scene,
/// that provides functionality for highlighting.
/// </summary>
public class CardContainer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer highlight;

    public void toggleHighlight(bool turnOn)
    {
        highlight.enabled = turnOn;
    }
}
