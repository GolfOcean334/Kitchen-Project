using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string title;
    public List<string> ingredients;
    public string instructions;
}

[System.Serializable]
public class SerializableRecipe
{
    public string title;
    public List<string> ingredients;
    public string instructions;

   
    public SerializableRecipe(Recipe recipe)
    {
        this.title = recipe.title;
        this.ingredients = new List<string>(recipe.ingredients);
        this.instructions = recipe.instructions;
    }
}

[System.Serializable]
public class SerializableRecipeList
{
    public List<SerializableRecipe> recipes;

    public SerializableRecipeList(List<Recipe> scriptableRecipes)
    {
        recipes = new List<SerializableRecipe>();
        foreach (var recipe in scriptableRecipes)
        {
            recipes.Add(new SerializableRecipe(recipe));
        }
    }
}
