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

    [SerializeField] private PickupHandler PickupHandler;

    private KitchenObject selectedIngredient;
    private Shelf currentShelf;
    private List<GameObject> currentIngredientSlots = new List<GameObject>();

    private void Start()
    {
        if (container.shelves == null || container.shelves.Count == 0)
        {
            Debug.LogWarning("Le conteneur ne contient aucune étagère !");
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
    private void AddObjectToShelf()
    {
        if (currentShelf == null)
        {
            Debug.LogWarning("Aucune étagère sélectionnée !");
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
        newIngredient.ingredientDescription = "Un nouvel ingrédient ajouté par le joueur.";
        newIngredient.prefab = handObject;
        newIngredient.image = handObject.GetComponentInChildren<SpriteRenderer>()?.sprite
                              ?? handObject.GetComponentInChildren<Image>()?.sprite;

        currentShelf.items.Add(newIngredient);

        PickupHandler.ClearCurrentMainHandObject();

        DisplayShelf(container.shelves.IndexOf(currentShelf));

        Debug.Log($"Un nouveau ScriptableObject {newIngredient.ingredientName} a été créé et ajouté à l’étagère.");
    }




    private void PickIngredient(string hand)
    {
        if (selectedIngredient == null)
        {
            Debug.LogWarning("Aucun ingrédient sélectionné !");
            return;
        }
        
    ingredientNameText.text = string.Empty;
    ingredientDescriptionText.text = string.Empty;

        GameObject ingredientObject = Instantiate(selectedIngredient.prefab);

        PickupHandler pickupHandler = FindObjectOfType<PickupHandler>();
        if (pickupHandler == null)
        {
            Debug.LogError("PickupHandler non trouvé dans la scène !");
            Destroy(ingredientObject);
            return;
        }

        if (hand == "LeftHand")
        {
            if (pickupHandler.GetLeftHandObject() == null)
            {
                pickupHandler.AttachObjectToHand(ingredientObject, pickupHandler.leftHand, ref pickupHandler.leftHandObject);
                pickupHandler.imgLeftHand.SetActive(false);
                Debug.Log($"Ingrédient {selectedIngredient.ingredientName} ajouté à la main gauche.");
            }
            else
            {
                Debug.LogWarning("La main gauche est déjà occupée !");
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
                Debug.Log($"Ingrédient {selectedIngredient.ingredientName} ajouté à la main droite.");
            }
            else
            {
                Debug.LogWarning("La main droite est déjà occupée !");
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
