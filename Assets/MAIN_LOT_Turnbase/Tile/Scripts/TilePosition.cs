using UnityEngine;

public class TilePosition
{
    private int _xPos;
    private int _zPos;
    public int Order;

    public TilePosition(int xPos, int zPos, int order)
    {
        _xPos = xPos;
        _zPos = zPos;
        Order = order;
    }

    public Vector3 GetPosition(float yPos, float multiplier)
    {
        return new Vector3(_xPos*multiplier, yPos, _zPos*multiplier);
    }

    public string PrintOut()
    {
        return $"X position: {_xPos}, Z position: {_zPos}, order: {Order}";
    }
}