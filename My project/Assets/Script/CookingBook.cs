using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;

public class CookingBook : MonoBehaviour
{
    [SerializeField] private List<Recipe> preloadedRecipes;
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();

    [SerializeField] private TMP_Text leftPageTitle;
    [SerializeField] private TMP_Text leftPageIngredients;
    [SerializeField] private TMP_Text leftPageInstructions;

    [SerializeField] private TMP_Text rightPageTitle;
    [SerializeField] private TMP_Text rightPageIngredients;
    [SerializeField] private TMP_Text rightPageInstructions;

    [SerializeField] private GameObject addRecipePanel;
    [SerializeField] private CanvasGroup bookCanvasGroup;
    [SerializeField] private TMP_InputField inputTitle;
    [SerializeField] private TMP_InputField inputIngredients;
    [SerializeField] private TMP_InputField inputInstructions;

    [SerializeField] private PlayerActions playerActions;
    private PlayerInput playerInput;
    private InputAction closeUICookingBook;

    [SerializeField] private Object saveFolder;

    private int currentPage = 0;

    private void Awake()
    {
        playerInput = new PlayerInput();
        closeUICookingBook = playerInput.Player.CloseUI;
    }
    private void OnEnable()
    {
        closeUICookingBook.Enable();
    }
    private void OnDisable()
    {
        closeUICookingBook.Disable();
    }
    private void Start()
    {
        LoadRecipesFromJSON();
        UpdateDisplay();
        addRecipePanel.SetActive(false);
    }
    private void Update()
    {
        if (closeUICookingBook.triggered)
        {
            HideAddRecipePanel();
        }
    }

    public void ShowAddRecipePanel()
    {
        if (playerActions != null)
        {
            playerActions.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        addRecipePanel.SetActive(true);
        if (bookCanvasGroup != null)
        {
            bookCanvasGroup.blocksRaycasts = false;
        }
    }

    public void HideAddRecipePanel()
    {
        if (playerActions != null)
        {
            playerActions.enabled = true;
        }
        addRecipePanel.SetActive(false);
        if (bookCanvasGroup != null)
        {
            bookCanvasGroup.blocksRaycasts = true;
        }
        ClearInputFields();
    }

    public void SaveNewRecipe()
    {
        string title = inputTitle.text.Trim();
        string ingredientsText = inputIngredients.text.Trim();
        string instructions = inputInstructions.text.Trim();

        if (!ValidateRecipe(title, ingredientsText, instructions))
        {
            Debug.LogError("Invalid recipe data. Please fill in all fields correctly.");
            return;
        }

        Recipe newRecipe = new Recipe
        {
            title = title,
            ingredients = new List<string>(ingredientsText.Split(", ")),
            instructions = instructions
        };

        recipes.Add(newRecipe);
        SaveRecipesToJSON();

        UpdateDisplay();
        HideAddRecipePanel();
    }

    private void SaveRecipesToJSON()
    {
        SerializableRecipeList serializableList = new SerializableRecipeList(this.recipes);
        string json = JsonUtility.ToJson(serializableList, true);

        string filePath = Application.dataPath + "/Data/recipes.json";

        string directoryPath = System.IO.Path.GetDirectoryName(filePath);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        System.IO.File.WriteAllText(filePath, json);

        Debug.Log("Recipes saved to " + filePath);
    }

    private void LoadRecipesFromJSON()
    {
        string filePath = Application.dataPath + "/Data/recipes.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            SerializableRecipeList loadedData = JsonUtility.FromJson<SerializableRecipeList>(json);
            recipes.Clear();
            foreach (var serializableRecipe in loadedData.recipes)
            {
                Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
                newRecipe.title = serializableRecipe.title;
                newRecipe.ingredients = new List<string>(serializableRecipe.ingredients);
                newRecipe.instructions = serializableRecipe.instructions;

                recipes.Add(newRecipe);
            }
            Debug.Log("Recipes loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("No recipes.json file found in " + filePath + ". Creating an empty recipe list.");
            recipes = new List<Recipe>();
        }
    }


    private bool ValidateRecipe(string title, string ingredientsText, string instructions)
    {
        if (string.IsNullOrEmpty(title)) return false;
        if (string.IsNullOrEmpty(ingredientsText)) return false;
        if (string.IsNullOrEmpty(instructions)) return false;

        return true;
    }

    private void LoadRecipes()
    {
        recipes.Clear();
        Recipe[] loadedRecipes = Resources.LoadAll<Recipe>("Recipes");
        foreach (var recipe in loadedRecipes)
        {
            if (!recipes.Contains(recipe))
            {
                recipes.Add(recipe);
            }
        }
    }

    public void AddARecipe(Recipe newRecipe)
    {
        if (newRecipe != null)
        {
            recipes.Add(newRecipe);
            UpdateDisplay();
        }
    }

    public void DeleteARecipe(Recipe recipeToRemove)
    {
        if (recipes.Contains(recipeToRemove))
        {
            recipes.Remove(recipeToRemove);
            currentPage = Mathf.Clamp(currentPage, 0, recipes.Count - 1);
            UpdateDisplay();
        }
    }

    public void NextPage()
    {
        if (currentPage < recipes.Count - 2)
        {
            currentPage += 2;
            UpdateDisplay();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage -= 2;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (recipes.Count == 0)
        {
            ClearPages();
            return;
        }

        if (currentPage < recipes.Count)
        {
            Recipe leftRecipe = recipes[currentPage];
            leftPageTitle.text = leftRecipe.title;
            leftPageIngredients.text = string.Join("\n", leftRecipe.ingredients);
            leftPageInstructions.text = leftRecipe.instructions;
        }
        else
        {
            ClearLeftPage();
        }

        if (currentPage + 1 < recipes.Count)
        {
            Recipe rightRecipe = recipes[currentPage + 1];
            rightPageTitle.text = rightRecipe.title;
            rightPageIngredients.text = string.Join("\n", rightRecipe.ingredients);
            rightPageInstructions.text = rightRecipe.instructions;
        }
        else
        {
            ClearRightPage();
        }
    }

    private void ClearPages()
    {
        ClearLeftPage();
        ClearRightPage();
    }

    private void ClearLeftPage()
    {
        leftPageTitle.text = "";
        leftPageIngredients.text = "";
        leftPageInstructions.text = "";
    }

    private void ClearRightPage()
    {
        rightPageTitle.text = "";
        rightPageIngredients.text = "";
        rightPageInstructions.text = "";
    }

    private void ClearInputFields()
    {
        inputTitle.text = "";
        inputIngredients.text = "";
        inputInstructions.text = "";
    }
}
