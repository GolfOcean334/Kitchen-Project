using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObject", menuName = "Item/KitchenObject")]
public class KitchenObject : ScriptableObject
{
    public string ingredientName;
    public string ingredientDescription;
    public GameObject prefab;
    public Sprite image;
}
