using UnityEngine;

public class ItemSelectionHandler : MonoBehaviour
{
    public HighlightHandler highlightHandler;  // R�f�rence au script HighlightHandler
    private GameObject[] collectibleItems;  // Liste des objets collect�s
    private int currentItemIndex = 0;  // Index de l'�l�ment actuellement s�lectionn�

    private void Start()
    {
        // R�cup�rer tous les objets collectables (vous pouvez ajuster ce code selon la structure de votre jeu)
        collectibleItems = GameObject.FindGameObjectsWithTag("Collectible");
    }

    private void Update()
    {
        // G�rer le scroll de la souris pour changer l'objet s�lectionn�
        HandleScrollInput();

        // Mettre � jour la surbrillance de l'objet s�lectionn�
        UpdateHighlight();
    }

    private void HandleScrollInput()
    {
        // D�tecter le mouvement de la molette de la souris
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Si la molette est tourn�e vers le haut (positive), s�lectionner l'�l�ment pr�c�dent
        if (scrollInput > 0f)
        {
            currentItemIndex = Mathf.Max(0, currentItemIndex - 1);  // S�lectionner un item pr�c�dent (en veillant � ne pas sortir du tableau)
        }
        // Si la molette est tourn�e vers le bas (n�gative), s�lectionner l'�l�ment suivant
        else if (scrollInput < 0f)
        {
            currentItemIndex = Mathf.Min(collectibleItems.Length - 1, currentItemIndex + 1);  // S�lectionner un item suivant (en veillant � ne pas d�passer la fin)
        }
    }

    private void UpdateHighlight()
    {
        // D�sactiver la surbrillance de tous les objets collect�s
        foreach (var item in collectibleItems)
        {
            if (item != null)
            {
                highlightHandler.ClearHighlight();
            }
        }

        // Activer la surbrillance de l'objet actuellement s�lectionn�
        if (collectibleItems.Length > 0 && collectibleItems[currentItemIndex] != null)
        {
            highlightHandler.ApplyHighlight(collectibleItems[currentItemIndex]);
        }
    }
}
