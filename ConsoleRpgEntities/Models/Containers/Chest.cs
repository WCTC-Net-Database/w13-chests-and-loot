namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// Chest - A container placed in the world that can hold items.
///
/// Chests are stored in the shared Containers table (TPH) with
/// ContainerType = "Chest". They are both IItemContainer (inherited from
/// Container) and ILockable - a common combination for world containers.
///
/// Lifecycle states:
///   - Locked + pickable  -> player can attempt to pick or use matching key
///   - Locked + unpickable + no key -> chest cannot be opened
///   - Trapped            -> first open attempt triggers the trap effect
///   - Open               -> items can be taken/added
///
/// Week 14 will add Room as a Container subclass. Rooms hold items on the
/// floor. Notice that nothing about Chest has to change when Room is added -
/// the Container base class handles all the shared concerns.
/// </summary>
public class Chest : Container, ILockable
{
    public string Description { get; set; } = string.Empty;

    // ============================================================
    // ILockable
    // ============================================================
    public bool IsLocked { get; set; }
    public bool IsTrapped { get; set; }
    public string? RequiredKeyId { get; set; }
    public bool IsPickable { get; set; } = true;

    /// <summary>
    /// True once the trap has been triggered or disarmed. Prevents the trap
    /// from firing repeatedly every time the chest is opened.
    /// </summary>
    public bool TrapDisarmed { get; set; }

    /// <summary>
    /// Amount of damage the trap deals when triggered.
    /// Zero means the chest has no damage trap.
    /// </summary>
    public int TrapDamage { get; set; }
}
