using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Weapon - Items that deal damage in combat.
/// Stored in the Items table with ItemType = "Weapon".
/// </summary>
public class Weapon : Item
{
    public int Attack { get; set; }

    /// <summary>
    /// Category for future weapon-specific logic (sword, bow, staff, etc.).
    /// Kept as a simple string for now - extend with an enum later if needed.
    /// </summary>
    public string Category { get; set; } = "Melee";

    /// <summary>
    /// Every weapon goes in the Weapon slot. No per-instance override needed.
    /// </summary>
    [NotMapped]
    public override SlotType? EligibleSlot => SlotType.Weapon;
}
