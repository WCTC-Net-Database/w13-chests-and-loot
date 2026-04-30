namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Armor - Items that reduce incoming damage.
/// Stored in the Items table with ItemType = "Armor".
///
/// The slot the armor goes in (Head, Body, Feet, Ring, ...) is set on the
/// inherited <see cref="Item.EligibleSlot"/> property — there's no separate
/// "Slot" string anymore. Equipping logic in Player.Equip looks up the
/// matching EquipmentSlot by SlotType.
/// </summary>
public class Armor : Item
{
    public int Defense { get; set; }
}
