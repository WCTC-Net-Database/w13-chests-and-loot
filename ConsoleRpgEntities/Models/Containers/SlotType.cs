namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// SlotType - the canonical set of equipment slots a player has.
///
/// =====================================================
/// WHY AN ENUM INSTEAD OF A STRING?
/// =====================================================
/// Up through W12, Armor.Slot was a plain string ("Head", "Body", "Feet", ...).
/// That worked, but a string has no compile-time safety: you could typo "head"
/// vs "Head", or introduce "HEAD" in seed data, and nothing would warn you
/// until a runtime bug surfaced.
///
/// Promoting the slot concept to an enum gives us:
///   - IntelliSense autocomplete when writing equip logic
///   - A compiler error if you typo the value
///   - An exhaustive `switch` in future code (the compiler can warn if you
///     add a new slot and forget to handle it somewhere)
///
/// =====================================================
/// WHY INTRODUCE IT NOW, IN W13?
/// =====================================================
/// W13 is about container subclasses having their own RULES:
///   - Chest has lock rules (you can't open a locked chest without a key)
///   - MonsterLoot has ownership rules (drops aren't yours until the monster dies)
///   - Equipment has SLOT rules (you can't equip two helmets at once)
///
/// The Equipment slot rule is the smallest of the three, so it makes a great
/// warm-up example before we dive into Chest and MonsterLoot. Same lesson,
/// smaller surface: a TPH subclass can enforce its own invariants.
/// </summary>
public enum SlotType
{
    Head,
    Body,
    Legs,
    Feet,
    Hands,
    Weapon,
    Shield,
    Ring,
    Accessory
}
