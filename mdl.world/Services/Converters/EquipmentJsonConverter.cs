using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using mdl.worlddata.Items;

namespace mdl.world.Services.Converters
{
    public class EquipmentJsonConverter : JsonConverter<Equipment>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Equipment).IsAssignableFrom(typeToConvert);
        }

        public override Equipment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Determine the type based on properties or a type discriminator
            if (root.TryGetProperty("weaponType", out _))
            {
                return JsonSerializer.Deserialize<Weapon>(root.GetRawText(), options) ?? new Weapon();
            }
            else if (root.TryGetProperty("magicType", out _))
            {
                return JsonSerializer.Deserialize<MagicalArtifact>(root.GetRawText(), options) ?? new MagicalArtifact();
            }
            else if (root.TryGetProperty("technologyType", out _))
            {
                return JsonSerializer.Deserialize<SciFiArtifact>(root.GetRawText(), options) ?? new SciFiArtifact();
            }

            // Default to a basic equipment implementation
            // Since Equipment is abstract, we'll create a concrete type
            return JsonSerializer.Deserialize<BasicEquipment>(root.GetRawText(), options) ?? new BasicEquipment();
        }

        public override void Write(Utf8JsonWriter writer, Equipment value, JsonSerializerOptions options)
        {
            // Add a type discriminator and serialize the actual type
            writer.WriteStartObject();
            
            // Write discriminator
            writer.WriteString("$type", value.GetType().Name);
            
            // Serialize all properties
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using var doc = JsonDocument.Parse(json);
            
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
            
            writer.WriteEndObject();
        }
    }

    // Concrete implementation for basic equipment
    public class BasicEquipment : Equipment
    {
        // This is a concrete class that can be instantiated
        public BasicEquipment()
        {
            Type = EquipmentType.Other;
        }
    }
}
