namespace ConsoleRpgEntities.Models.Containers;

/// <summary>
/// ILockable - A contract for anything in the game world that can be locked,
/// trapped, and unlocked.
///
/// Week 13 implements this on Chest.
/// Week 14 will implement this on Door - same interface, same unlock logic,
/// completely different entity. That's the Open/Closed Principle at work:
/// your Unlock/Pick code doesn't care whether you're opening a chest or a door.
///
/// Notice how this is a separate interface from IItemContainer. A Chest is
/// both IItemContainer (it holds items) and ILockable (it can be locked).
/// But a Door will be ILockable without being IItemContainer - a door doesn't
/// hold items. Splitting these concerns follows the Interface Segregation
/// Principle (ISP).
/// </summary>
public interface ILockable
{
    /// <summary>
    /// True if the entity is currently locked and blocks access.
    /// </summary>
    bool IsLocked { get; set; }

    /// <summary>
    /// True if opening the entity triggers a trap. Usually paired with a
    /// trap effect the implementing class decides how to apply.
    /// </summary>
    bool IsTrapped { get; set; }

    /// <summary>
    /// Optional tag that identifies the specific key needed to unlock this
    /// entity. When null, any KeyItem can attempt the lock. When set, only
    /// a KeyItem whose KeyId matches will succeed.
    /// </summary>
    string? RequiredKeyId { get; set; }

    /// <summary>
    /// True if the lock can be picked with a lockpick-style KeyItem
    /// (a KeyItem where KeyId is null). Some locks are pick-proof.
    /// </summary>
    bool IsPickable { get; set; }
}
