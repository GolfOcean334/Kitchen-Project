using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Container : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Liste des �tag�res du conteneur (max 3 �tag�res)")]
    public List<Shelf> shelves = new List<Shelf>(3);

    private void OnValidate()
    {
        if (shelves.Count > 3)
        {
            Debug.LogWarning("Un conteneur ne peut avoir que 3 �tag�res au maximum.");
            shelves = shelves.GetRange(0, 3);
        }

        while (shelves.Count < 3)
        {
            shelves.Add(new Shelf { shelfName = $"Shelf {shelves.Count + 1}" });
        }

        foreach (var shelf in shelves)
        {
            if (shelf.items == null)
            {
                shelf.items = new List<KitchenObject>();
            }
        }
    }
}
