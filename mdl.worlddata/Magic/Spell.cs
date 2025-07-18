using System;
using System.Collections.Generic;

namespace mdl.worlddata.Magic
{
    // Spell books and magic
    public class SpellBook
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty; // Historic figure
        public Items.MagicType MagicSchool { get; set; }
        public int RequiredLevel { get; set; }
        public List<Spell> Spells { get; set; } = new List<Spell>();
        public string Language { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = true;
        public List<string> MissingPages { get; set; } = new List<string>();
    }

    public class Spell
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public Items.MagicType School { get; set; }
        public string Components { get; set; } = string.Empty;
        public string CastingTime { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public List<string> Effects { get; set; } = new List<string>();
        public bool IsRitual { get; set; } = false;
    }
}
