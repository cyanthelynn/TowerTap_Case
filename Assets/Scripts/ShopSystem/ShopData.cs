using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Config/ShopData")]
public class ShopData : ScriptableObject
{
    public List<ShopDefinition> shopDefinitions = new List<ShopDefinition>();
}

[System.Serializable]
public struct ShopDefinition
{
    public int price;
    public Color color;
}