namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Equipment - A character's set of wearable slots (Head, Body, Weapon, etc.).
///
/// Equipment is a Container (so equipped items still live in the Items table
/// via ContainerId pointing here), AND it owns a set of <see cref="EquipmentSlot"/>
/// rows that record which item is in which slot. The collection-of-slots design
/// lets us enforce "one item per slot" cleanly in <see cref="Characters.Player.Equip"/>.
///
/// Querying "what's my equipped weapon?" still works through the flat collection:
///
///     var weapon = player.Equipment.Items.OfType&lt;Weapon&gt;().FirstOrDefault();
///
/// Or via the slots if you want slot-aware access:
///
///     var weaponSlot = player.Equipment.EquipmentSlots
///         .FirstOrDefault(s => s.SlotType == SlotType.Weapon);
///     var weapon = weaponSlot?.EquippedItem as Weapon;
/// </summary>
public class Equipment : Container
{
    public virtual ICollection<EquipmentSlot> EquipmentSlots { get; set; } = new List<EquipmentSlot>();
}
