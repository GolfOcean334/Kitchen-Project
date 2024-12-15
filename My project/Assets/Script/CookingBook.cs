using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class CookingBook : MonoBehaviour
{
    public List<Recipe> preloadedRecipes;
    public List<Recipe> recipes = new List<Recipe>();

    public TMP_Text leftPageTitle;
    public TMP_Text leftPageIngredients;
    public TMP_Text leftPageInstructions;

    public TMP_Text rightPageTitle;
    public TMP_Text rightPageIngredients;
    public TMP_Text rightPageInstructions;

    public GameObject addRecipePanel;
    public CanvasGroup bookCanvasGroup;
    public TMP_InputField inputTitle;
    public TMP_InputField inputIngredients;
    public TMP_InputField inputInstructions;

    public PlayerActions playerActions;

    public Object saveFolder;

    private int currentPage = 0;

    private void Start()
    {
        LoadRecipes();
        UpdateDisplay();
        addRecipePanel.SetActive(false);
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

        Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.title = title;
        newRecipe.ingredients = new List<string>(ingredientsText.Split(' '));
        newRecipe.instructions = instructions;

        string folderPath = AssetDatabase.GetAssetPath(saveFolder);
        if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Save folder is not valid. Please set a valid folder in the inspector.");
            return;
        }

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + title + ".asset");
        AssetDatabase.CreateAsset(newRecipe, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        recipes.Add(newRecipe);
        UpdateDisplay();
        HideAddRecipePanel();
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
