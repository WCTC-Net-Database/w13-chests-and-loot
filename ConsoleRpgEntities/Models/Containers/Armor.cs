using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Armor - Items that reduce incoming damage.
/// Stored in the Items table with ItemType = "Armor".
/// </summary>
public class Armor : Item
{
    public int Defense { get; set; }

    /// <summary>
    /// Slot the armor occupies when equipped, stored as a string in the DB
    /// ("Head", "Body", "Hands", "Feet", "Ring", ...). Kept as a string so
    /// existing seed data works without a migration.
    ///
    /// The typed, enum-based view of this value is exposed via EligibleSlot
    /// below - use that for equip-time logic; use this for DB storage.
    /// </summary>
    public string Slot { get; set; } = "Body";

    /// <summary>
    /// Parsed, enum-typed view of the Slot string. Falls back to Body if the
    /// string doesn't match any known slot (shouldn't happen in practice -
    /// the seed data is controlled - but defensive coding is free here).
    /// </summary>
    [NotMapped]
    public override SlotType? EligibleSlot =>
        Enum.TryParse<SlotType>(Slot, ignoreCase: true, out var parsed)
            ? parsed
            : SlotType.Body;
}
