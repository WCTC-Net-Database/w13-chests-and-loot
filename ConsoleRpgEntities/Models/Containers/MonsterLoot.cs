using ConsoleRpgEntities.Models.Characters.Monsters;

namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// MonsterLoot - A container attached to a monster. When the monster is alive,
/// the loot is unreachable. When the monster is defeated, the player can loot
/// the container and the items transfer to their inventory.
///
/// Stored in the Containers table with ContainerType = "MonsterLoot".
///
/// A monster has a one-to-one relationship with its MonsterLoot container:
/// each monster carries its own loot. The relationship is owned by the
/// Monster entity (Monster.LootId is the foreign key).
/// </summary>
public class MonsterLoot : Container
{
    // No back-reference to Monster needed - the FK lives on Monster.LootId.
    // This mirrors how Player -> Inventory/Equipment is modeled in W12.
}
