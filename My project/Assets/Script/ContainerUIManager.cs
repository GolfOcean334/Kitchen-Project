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
    [SerializeField] private Button[] shelfButtons;

    [SerializeField] private PickupHandler PickupHandler;
    private KitchenObject selectedIngredient;
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

        Shelf shelf = container.shelves[shelfIndex];

        foreach (var item in shelf.items)
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

    private void PickIngredient(string hand)
    {
        if (selectedIngredient == null) return;

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
                Debug.Log($"Ingrédient {selectedIngredient.ingredientName} ajouté à la main gauche.");
            }
            else
            {
                Debug.LogWarning("La main gauche est déjà occupée !");
                Destroy(ingredientObject);
            }
        }
        else if (hand == "RightHand")
        {
            if (pickupHandler.GetRightHandObject() == null)
            {
                pickupHandler.AttachObjectToHand(ingredientObject, pickupHandler.rightHand, ref pickupHandler.rightHandObject);
                Debug.Log($"Ingrédient {selectedIngredient.ingredientName} ajouté à la main droite.");
            }
            else
            {
                Debug.LogWarning("La main droite est déjà occupée !");
                Destroy(ingredientObject);
            }
        }

        ingredientNameText.text = string.Empty;
        ingredientDescriptionText.text = string.Empty;
        ingredientImage.sprite = null;
        selectedIngredient = null;
    }


}
