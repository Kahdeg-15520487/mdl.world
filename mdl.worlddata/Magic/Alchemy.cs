using System;
using System.Collections.Generic;

namespace mdl.worlddata.Magic
{
    // Alchemy
    public class AlchemyRecipe
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AlchemyType Type { get; set; }
        public int Difficulty { get; set; } // 1-10 scale
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<string> Steps { get; set; } = new List<string>();
        public TimeSpan PreparationTime { get; set; }
        public List<string> Effects { get; set; } = new List<string>();
        public List<string> SideEffects { get; set; } = new List<string>();
        public string CreatorId { get; set; } = string.Empty;
        public bool IsSecret { get; set; } = false;
    }

    public class Ingredient
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public IngredientRarity Rarity { get; set; }
        public string Source { get; set; } = string.Empty;
        public List<string> Properties { get; set; } = new List<string>();
    }

    public enum AlchemyType
    {
        Healing,
        Poison,
        Enhancement,
        Transformation,
        Utility,
        Combat,
        Divination,
        Other
    }

    public enum IngredientRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
