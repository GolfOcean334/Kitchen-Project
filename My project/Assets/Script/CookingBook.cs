using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Recipe", menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string title;
    public List<string> ingredients;
    public string instructions;
}

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
    public LockCursor lockCursor;

    private int currentPage = 0;

    private void Start()
    {
        LoadPreloadedRecipes();
        UpdateDisplay();
        addRecipePanel.SetActive(false);
    }

    private void LoadPreloadedRecipes()
    {
        foreach (var recipe in preloadedRecipes)
        {
            if (recipe != null && !recipes.Contains(recipe))
            {
                recipes.Add(recipe);
            }
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
        string title = inputTitle.text;
        string ingredientsText = inputIngredients.text;
        string instructions = inputInstructions.text;

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(ingredientsText) && !string.IsNullOrEmpty(instructions))
        {
            Recipe newRecipe = ScriptableObject.CreateInstance<Recipe>();
            newRecipe.title = title;
            newRecipe.ingredients = new List<string>(ingredientsText.Split('\n'));
            newRecipe.instructions = instructions;

            recipes.Add(newRecipe);
            UpdateDisplay();
            HideAddRecipePanel();
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
