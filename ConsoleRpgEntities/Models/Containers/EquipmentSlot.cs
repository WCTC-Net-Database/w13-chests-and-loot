namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// EquipmentSlot - one wearable position on a Character (Head, Body, Weapon, etc.).
///
/// Each Equipment container owns a fixed set of EquipmentSlots — one per
/// SlotType. <see cref="EquippedItem"/> (nullable) is the Item currently
/// occupying the slot, or null if the slot is empty.
///
/// =====================================================
/// WHY AN ENTITY INSTEAD OF A FLAT LIST?
/// =====================================================
/// Last week, Equipment was effectively just a bag of equipped items - all
/// equipped weapons and armor sat together in Equipment.Items, indistinguishable
/// from each other except by type. That's fine for "what's my total attack
/// bonus?" but it can't enforce "you can't wear two helmets at once" without
/// extra runtime checks.
///
/// EquipmentSlot makes the slot itself a first-class entity:
///   - One row per slot per character (so a character has exactly one Head slot)
///   - The slot's <see cref="EquippedItem"/> FK either points at an item or is null
///   - Trying to equip a second item in an occupied slot is now a constraint
///     violation we can detect cleanly in <see cref="Characters.Player.Equip"/>
///
/// You'll see this same "promote a value to an entity" pattern any time a
/// concept gains state of its own (here: which item fills it).
/// </summary>
public class EquipmentSlot
{
    public int Id { get; set; }

    /// <summary>
    /// Which slot this is (Head, Body, Weapon, ...). Stored as int by EF Core.
    /// </summary>
    public SlotType SlotType { get; set; }

    /// <summary>
    /// The item currently occupying this slot. Null = slot is empty.
    /// </summary>
    public int? EquippedItemId { get; set; }
    public virtual Item? EquippedItem { get; set; }

    /// <summary>
    /// FK back to the Equipment container that owns this slot.
    /// </summary>
    public int? EquipmentId { get; set; }
    public virtual Equipment? Equipment { get; set; }
}
