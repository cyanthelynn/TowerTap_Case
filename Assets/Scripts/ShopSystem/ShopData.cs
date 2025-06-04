using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Config/ShopData")]
public class ShopData : ScriptableObject
{
    public List<ShopDefinition> shopDefinitions = new List<ShopDefinition>();
}

[Serializable]
public struct ShopDefinition
{
    public Sprite icon;
    public int price;
}