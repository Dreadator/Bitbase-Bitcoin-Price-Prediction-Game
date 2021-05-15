using UnityEngine;

// Interface For The Graph Visuals, All graphs must adhere to these Method Signatures.
public interface IGraphVisual 
{
    GameObject AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string price);
}
