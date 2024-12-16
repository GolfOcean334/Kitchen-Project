using UnityEngine;

public class ItemSelectionHandler : MonoBehaviour
{
    public HighlightHandler highlightHandler;  // Référence au script HighlightHandler
    private GameObject[] collectibleItems;  // Liste des objets collectés
    private int currentItemIndex = 0;  // Index de l'élément actuellement sélectionné

    private void Start()
    {
        // Récupérer tous les objets collectables (vous pouvez ajuster ce code selon la structure de votre jeu)
        collectibleItems = GameObject.FindGameObjectsWithTag("Collectible");
    }

    private void Update()
    {
        // Gérer le scroll de la souris pour changer l'objet sélectionné
        HandleScrollInput();

        // Mettre à jour la surbrillance de l'objet sélectionné
        UpdateHighlight();
    }

    private void HandleScrollInput()
    {
        // Détecter le mouvement de la molette de la souris
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Si la molette est tournée vers le haut (positive), sélectionner l'élément précédent
        if (scrollInput > 0f)
        {
            currentItemIndex = Mathf.Max(0, currentItemIndex - 1);  // Sélectionner un item précédent (en veillant à ne pas sortir du tableau)
        }
        // Si la molette est tournée vers le bas (négative), sélectionner l'élément suivant
        else if (scrollInput < 0f)
        {
            currentItemIndex = Mathf.Min(collectibleItems.Length - 1, currentItemIndex + 1);  // Sélectionner un item suivant (en veillant à ne pas dépasser la fin)
        }
    }

    private void UpdateHighlight()
    {
        // Désactiver la surbrillance de tous les objets collectés
        foreach (var item in collectibleItems)
        {
            if (item != null)
            {
                highlightHandler.ClearHighlight();
            }
        }

        // Activer la surbrillance de l'objet actuellement sélectionné
        if (collectibleItems.Length > 0 && collectibleItems[currentItemIndex] != null)
        {
            highlightHandler.ApplyHighlight(collectibleItems[currentItemIndex]);
        }
    }
}
