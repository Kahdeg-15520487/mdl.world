using System;
using System.Collections.Generic;

namespace mdl.worlddata.Items
{
    // Equipment and artifacts
    public abstract class Equipment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EquipmentType Type { get; set; }
        public EquipmentRarity Rarity { get; set; }
        public double Weight { get; set; } // in kg
        public decimal Value { get; set; } // in gold pieces or credits
        public string Material { get; set; } = string.Empty;
        public string Condition { get; set; } = "Good";
        public string? CreatorId { get; set; } // Historic figure who created it
        public string? CurrentOwnerId { get; set; } // Current owner
        public List<string> History { get; set; } = new List<string>();
        public Dictionary<string, int> Properties { get; set; } = new Dictionary<string, int>();
    }

    public class Weapon : Equipment
    {
        public WeaponType WeaponType { get; set; }
        public int Damage { get; set; }
        public int Range { get; set; }
        public string DamageType { get; set; } = string.Empty;
        public bool IsMagical { get; set; } = false;
        public List<string> Enchantments { get; set; } = new List<string>();
    }

    public class MagicalArtifact : Equipment
    {
        public MagicType MagicType { get; set; }
        public int MagicPower { get; set; }
        public List<string> Spells { get; set; } = new List<string>();
        public int Charges { get; set; }
        public int MaxCharges { get; set; }
        public bool RequiresAttunement { get; set; } = false;
        public string ActivationMethod { get; set; } = string.Empty;
    }

    public class SciFiArtifact : Equipment
    {
        public TechnologyType TechnologyType { get; set; }
        public int TechLevel { get; set; }
        public string PowerSource { get; set; } = string.Empty;
        public int PowerLevel { get; set; }
        public List<string> Functions { get; set; } = new List<string>();
        public bool IsOperational { get; set; } = true;
        public string OperatingSystem { get; set; } = string.Empty;
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
        Tool,
        Consumable,
        Artifact,
        Jewelry,
        Book,
        Container,
        Other
    }

    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical,
        Unique
    }

    public enum WeaponType
    {
        Sword,
        Axe,
        Mace,
        Bow,
        Crossbow,
        Spear,
        Dagger,
        Staff,
        Wand,
        Firearm,
        EnergyWeapon,
        Other
    }

    public enum MagicType
    {
        Arcane,
        Divine,
        Nature,
        Elemental,
        Necromantic,
        Illusion,
        Enchantment,
        Transmutation,
        Other
    }

    public enum TechnologyType
    {
        Weapon,
        Communication,
        Transportation,
        Medical,
        Computing,
        Energy,
        Manufacturing,
        Defense,
        Other
    }
}
