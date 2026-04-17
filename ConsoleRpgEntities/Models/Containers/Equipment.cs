namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Equipment - A player's equipped item slots (weapon, armor, etc.).
///
/// This is a Container subclass - all equipped items live in the same Items table
/// with ContainerId pointing at this Equipment row. Querying "what's equipped" is:
///
///     var weapon = player.Equipment.Items.OfType&lt;Weapon&gt;().FirstOrDefault();
///     var armor  = player.Equipment.Items.OfType&lt;Armor&gt;().FirstOrDefault();
///
/// This is much simpler than Week 11's separate WeaponId/ArmorId foreign keys.
///
/// =====================================================
/// WARM-UP FOR THIS WEEK'S LESSON
/// =====================================================
/// Equipment is the first Container subclass that enforces a RULE of its own:
/// you can't equip two items in the same slot. That's a tiny example of a
/// much bigger theme this week - container subclasses earning their keep by
/// imposing constraints the base class doesn't know about.
///
/// The CanEquip() method below is the rule. W13 continues the theme:
///   - Chest.IsLocked enforces that you need the right key to open it
///   - MonsterLoot is read-only until the monster dies
///
/// Same interface (IItemContainer / Container base), different enforcement
/// per subclass. That's the Liskov Substitution Principle applied at the
/// container level.
/// </summary>
public class Equipment : Container
{
    /// <summary>
    /// Returns true if <paramref name="item"/> can be equipped right now.
    /// Two rules:
    ///   1. The item must be equippable at all (EligibleSlot not null -
    ///      rules out Consumables and KeyItems).
    ///   2. No other item currently in this Equipment must already occupy
    ///      the same slot.
    ///
    /// Callers (like Player.Equip or a UI layer) should consult this before
    /// calling AddItem. A more paranoid design would override AddItem itself
    /// and throw - we leave that as a judgment call for the student.
    /// </summary>
    public bool CanEquip(Item item)
    {
        if (item.EligibleSlot == null) return false;
        return !Items.Any(existing => existing.EligibleSlot == item.EligibleSlot);
    }
}
