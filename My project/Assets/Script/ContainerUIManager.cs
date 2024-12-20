using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUIManager : MonoBehaviour
{
    [SerializeField] private Container container;
    [SerializeField] private Transform ingredientGridParent;
    [SerializeField] private GameObject ingredientSlotPrefab;
    [SerializeField] private TextMeshProUGUI ingredientNameText;
    [SerializeField] private TextMeshProUGUI ingredientDescriptionText;
    [SerializeField] private Image ingredientImage;
    [SerializeField] private Button leftHandButton;
    [SerializeField] private Button rightHandButton;
    [SerializeField] private Button addToShelfButton;
    [SerializeField] private Button[] shelfButtons;

    [SerializeField] private Button randomizeButton;
    private bool isRandomized = false;
    private List<Shelf> originalShelves;

    [SerializeField] private PickupHandler PickupHandler;

    private KitchenObject selectedIngredient;
    private Shelf currentShelf;
    private List<GameObject> currentIngredientSlots = new List<GameObject>();

    private void Start()
    {
        if (container.shelves == null || container.shelves.Count == 0)
        {
            Debug.LogWarning("Le conteneur ne contient aucune �tag�re !");
            return;
        }

        for (int i = 0; i < shelfButtons.Length; i++)
        {
            if (i >= container.shelves.Count)
            {
                shelfButtons[i].gameObject.SetActive(false);
            }
            else
            {
                int shelfIndex = i;
                shelfButtons[i].gameObject.SetActive(true);
                shelfButtons[i].onClick.RemoveAllListeners();
                shelfButtons[i].onClick.AddListener(() => DisplayShelf(shelfIndex));
            }
        }

        // Ajouter les listeners pour les boutons des mains
        leftHandButton.onClick.RemoveAllListeners();
        leftHandButton.onClick.AddListener(() => PickIngredient("LeftHand"));

        rightHandButton.onClick.RemoveAllListeners();
        rightHandButton.onClick.AddListener(() => PickIngredient("RightHand"));

        addToShelfButton.onClick.RemoveAllListeners();
        addToShelfButton.onClick.AddListener(AddObjectToShelf);

        randomizeButton.onClick.RemoveAllListeners();
        randomizeButton.onClick.AddListener(ToggleRandomizeContainer);

        DisplayShelf(0);
    }

    public void DisplayShelf(int shelfIndex)
    {
        foreach (var slot in currentIngredientSlots)
        {
            Destroy(slot);
        }
        currentIngredientSlots.Clear();

        if (shelfIndex < 0 || shelfIndex >= container.shelves.Count) return;

        currentShelf = container.shelves[shelfIndex];

        foreach (var item in currentShelf.items)
        {
            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientGridParent);
            currentIngredientSlots.Add(slot);

            Image slotImage = slot.GetComponentInChildren<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = item.image;
            }

            Button slotButton = slot.GetComponentInChildren<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => ShowIngredientDetails(item));
            }
        }
    }

    private void ShowIngredientDetails(KitchenObject ingredient)
    {
        selectedIngredient = ingredient;
        ingredientNameText.text = ingredient.ingredientName;
        ingredientDescriptionText.text = ingredient.ingredientDescription;
    }

    public void ToggleRandomizeContainer()
    {
        if (!isRandomized)
        {
            // Sauvegarder l'�tat d'origine
            originalShelves = new List<Shelf>();
            foreach (var shelf in container.shelves)
            {
                var copy = new Shelf
                {
                    items = new List<KitchenObject>(shelf.items)
                };
                originalShelves.Add(copy);
            }

            // Randomiser le nombre d'�tag�res et les ingr�dients
            int shelfCount = Random.Range(0, 4); // Nombre d'�tag�res entre 0 et 3
            container.shelves = new List<Shelf>();
            for (int i = 0; i < shelfCount; i++)
            {
                Shelf randomShelf = new Shelf { items = new List<KitchenObject>() };
                int ingredientCount = Random.Range(1, 16); // Nombre d'ingr�dients entre 1 et 15
                for (int j = 0; j < ingredientCount; j++)
                {
                    if (originalShelves.Count > 0)
                    {
                        int randomShelfIndex = Random.Range(0, originalShelves.Count);
                        Shelf sourceShelf = originalShelves[randomShelfIndex];
                        if (sourceShelf.items.Count > 0)
                        {
                            int randomItemIndex = Random.Range(0, sourceShelf.items.Count);
                            KitchenObject randomItem = sourceShelf.items[randomItemIndex];
                            randomShelf.items.Add(randomItem);
                        }
                    }
                }
                container.shelves.Add(randomShelf);
            }

            // Mettre � jour les boutons d'�tag�re en fonction des �tag�res qui contiennent des ingr�dients
            UpdateShelfButtons();

            isRandomized = true;
        }
        else
        {
            // Restaurer l'�tat d'origine
            container.shelves = originalShelves;
            originalShelves = null;

            // Mettre � jour les boutons d'�tag�re en fonction des �tag�res d'origine
            UpdateShelfButtons();

            isRandomized = false;
        }

        // Afficher la premi�re �tag�re apr�s modification
        DisplayShelf(0);
    }

    private void UpdateShelfButtons()
    {
        for (int i = 0; i < shelfButtons.Length; i++)
        {
            if (i < container.shelves.Count && container.shelves[i].items.Count > 0)
            {
                // L'�tag�re existe et contient des ingr�dients, afficher le bouton
                shelfButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // L'�tag�re ne contient pas d'ingr�dients, masquer le bouton
                shelfButtons[i].gameObject.SetActive(false);
            }
        }
    }


    private void AddObjectToShelf()
    {
        if (currentShelf == null)
        {
            Debug.LogWarning("Aucune �tag�re s�lectionn�e !");
            return;
        }

        GameObject handObject = PickupHandler.GetCurrentMainHandObject();
        if (handObject == null)
        {
            Debug.LogWarning("Aucun objet n'est tenu dans les mains !");
            return;
        }

        KitchenObject newIngredient = ScriptableObject.CreateInstance<KitchenObject>();
        newIngredient.ingredientName = handObject.name;
        newIngredient.ingredientDescription = "Un nouvel ingr�dient ajout� par le joueur.";
        newIngredient.prefab = handObject;
        newIngredient.image = handObject.GetComponentInChildren<SpriteRenderer>()?.sprite
                              ?? handObject.GetComponentInChildren<Image>()?.sprite;

        currentShelf.items.Add(newIngredient);

        PickupHandler.ClearCurrentMainHandObject();

        DisplayShelf(container.shelves.IndexOf(currentShelf));

        Debug.Log($"Un nouveau ScriptableObject {newIngredient.ingredientName} a �t� cr�� et ajout� � l��tag�re.");
    }




    private void PickIngredient(string hand)
    {
        if (selectedIngredient == null)
        {
            Debug.LogWarning("Aucun ingr�dient s�lectionn� !");
            return;
        }
        
    ingredientNameText.text = string.Empty;
    ingredientDescriptionText.text = string.Empty;

        GameObject ingredientObject = Instantiate(selectedIngredient.prefab);

        PickupHandler pickupHandler = FindObjectOfType<PickupHandler>();
        if (pickupHandler == null)
        {
            Debug.LogError("PickupHandler non trouv� dans la sc�ne !");
            Destroy(ingredientObject);
            return;
        }

        if (hand == "LeftHand")
        {
            if (pickupHandler.GetLeftHandObject() == null)
            {
                pickupHandler.AttachObjectToHand(ingredientObject, pickupHandler.leftHand, ref pickupHandler.leftHandObject);
                pickupHandler.imgLeftHand.SetActive(false);
                Debug.Log($"Ingr�dient {selectedIngredient.ingredientName} ajout� � la main gauche.");
            }
            else
            {
                Debug.LogWarning("La main gauche est d�j� occup�e !");
                Destroy(ingredientObject);
                return;
            }
        }
        else if (hand == "RightHand")
        {
            if (pickupHandler.GetRightHandObject() == null)
            {
                pickupHandler.AttachObjectToHand(ingredientObject, pickupHandler.rightHand, ref pickupHandler.rightHandObject);
                pickupHandler.imgRightHand.SetActive(false);
                Debug.Log($"Ingr�dient {selectedIngredient.ingredientName} ajout� � la main droite.");
            }
            else
            {
                Debug.LogWarning("La main droite est d�j� occup�e !");
                Destroy(ingredientObject);
                return;
            }
        }

        if (currentShelf != null)
        {
            currentShelf.items.Remove(selectedIngredient);
            DisplayShelf(container.shelves.IndexOf(currentShelf));
        }

        ingredientNameText.text = string.Empty;
        ingredientDescriptionText.text = string.Empty;
        selectedIngredient = null;
    }
}
