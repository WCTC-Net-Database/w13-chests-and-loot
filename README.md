# Week 13: Chests & Monster Loot

> **Template Purpose:** This template represents a working solution through Week 12. Use YOUR repo if you're caught up. Use this as a fresh start if needed.

---

## Overview

This week you'll extend the `Container` hierarchy from Week 12 with **two new container types** and see the Open/Closed Principle pay off:

1. **`Chest`** — placed in the world, can be **locked**, **trapped**, and **pickable**, and holds items waiting to be looted.
2. **`MonsterLoot`** — attached to a monster, unreachable while the monster is alive, and lootable after the monster is defeated.

You'll also meet a new interface, **`ILockable`**, which describes "anything that can be locked and trapped." Week 14 will reuse this exact same interface on **doors**, so your unlock/pick logic is already doing double duty.

> **Continuity from W12:** you don't rewrite any existing code. Inventory, Equipment, Item, Weapon/Armor/Consumable/KeyItem, and all the Week 12 LINQ methods stay exactly as they are. That's the Open/Closed Principle in action — the system was **open for extension** but **closed for modification**.

## Learning Objectives

By completing this assignment, you will:
- [ ] Extend a TPH hierarchy with new subclasses without changing existing rows
- [ ] Apply the Open/Closed Principle in a real EF Core model change
- [ ] Separate two concerns with two interfaces (`IItemContainer` + `ILockable`)
- [ ] Implement state-based logic (locked → unlocked, trapped → disarmed)
- [ ] Work with an `enum` return type to model multiple outcomes cleanly
- [ ] Use LINQ `OfType<T>()` to query a TPH hierarchy by concrete type
- [ ] Write and run two migrations — one for schema, one for seed data

## Prerequisites

- [ ] Completed Week 12 assignment (or using this template)
- [ ] Understanding of Container/Item TPH from W12
- [ ] Working LINQ including `OfType<T>`

---

## What's New This Week

| Concept | Description |
|---------|-------------|
| `Chest` | New Container subclass with lock/trap/pickable state |
| `MonsterLoot` | New Container subclass attached to monsters, looted on defeat |
| `ILockable` | Interface for anything that can be locked, trapped, and picked |
| `Player.OpenChest` | Returns an `enum` describing what happened (Opened, Locked, Trapped, AlreadyOpen) |
| `Player.TryUnlock` | Handles both lockpicks (break on use) and specific keys |
| `Player.LootChest` / `LootMonster` | Transfers items between containers |
| `Monster.LootId` | One-to-one relationship from Monster to MonsterLoot container |

---

## Warm-Up: Equipment Earns Its First Rule

Before we get to chests, look at `Models/Containers/Equipment.cs`. Last week it was an empty shell — a Container subclass that did nothing special. This week it gets its first invariant:

```csharp
public bool CanEquip(Item item)
{
    if (item.EligibleSlot == null) return false;
    return !Items.Any(existing => existing.EligibleSlot == item.EligibleSlot);
}
```

Two new pieces support this:
- A `SlotType` enum (Head, Body, Hands, Weapon, Shield, ...) — enum instead of string gives compile-time safety
- A virtual `Item.EligibleSlot` property that Weapon and Armor override, defaulting to `null` on items that can't be equipped (Consumables, KeyItems)

No migration needed — `EligibleSlot` is `[NotMapped]` and derived from existing data. This is the **smallest possible example** of the week's big idea: a Container subclass enforcing a rule its base doesn't know about. Chest and MonsterLoot are the same idea at larger scale.

---

## The Big Idea: Open/Closed Principle in Action

Last week you built `Container` as an abstract base class with `Inventory` and `Equipment` subclasses. This week, adding chests and monster loot is essentially a **five-line change** to `GameContext.OnModelCreating`:

```csharp
modelBuilder.Entity<Container>()
    .HasDiscriminator<string>(c => c.ContainerType)
    .HasValue<Inventory>("Inventory")     // from W12
    .HasValue<Equipment>("Equipment")     // from W12
    .HasValue<Chest>("Chest")             // NEW
    .HasValue<MonsterLoot>("MonsterLoot"); // NEW
```

That's it. The `Items` table doesn't change. The `Inventory` table... doesn't even exist. All containers share the one `Containers` table via TPH, so adding new kinds of containers is **additive only** — no existing code needs to be modified.

This is the Open/Closed Principle (the O in SOLID): **open for extension, closed for modification**. You'll see the same pattern again in Week 14 when we add `Room` as yet another Container subclass.

---

## Two Interfaces, One Entity

Notice that `Chest` implements **two** interfaces:

```csharp
public class Chest : Container, ILockable
{
    // IItemContainer comes from Container (inherited)
    // ILockable adds: IsLocked, IsTrapped, RequiredKeyId, IsPickable
}
```

This is **Interface Segregation** (the I in SOLID). Rather than one giant `IChest` interface that bundles "holds items" and "can be locked" together, we split them:

- **`IItemContainer`** — anything that holds items (Inventory, Equipment, Chest, MonsterLoot, Room later)
- **`ILockable`** — anything that can be locked and trapped (Chest now, Door in W14)

A `MonsterLoot` implements `IItemContainer` but NOT `ILockable` — you can't lock a corpse. A future `Door` will implement `ILockable` but NOT `IItemContainer` — a door doesn't hold items. Splitting the interfaces means each entity only declares the capabilities it actually has.

---

## Project Structure

```
W13-assignment-template.sln
│
├── ConsoleRpg/                           # UI & Game Logic
│   ├── Program.cs
│   ├── Startup.cs
│   ├── appsettings.json
│   ├── Services/
│   │   └── GameEngine.cs                 # NEW: chest menu, combat-and-loot flow
│   └── Helpers/
│       ├── MenuManager.cs
│       └── OutputManager.cs
│
└── ConsoleRpgEntities/                   # Data & Models
    ├── Data/
    │   ├── GameContext.cs                # Extended: Chest/MonsterLoot discriminators
    │   └── GameContextFactory.cs
    ├── Models/
    │   ├── Characters/
    │   │   ├── Player.cs                 # NEW: OpenChest, TryUnlock, LootChest, LootMonster
    │   │   └── Monsters/
    │   │       ├── Monster.cs            # NEW: LootId, IsLooted
    │   │       └── Goblin.cs
    │   ├── Containers/
    │   │   ├── IItemContainer.cs         # From W12
    │   │   ├── ILockable.cs              # NEW: Lock/trap/pick contract
    │   │   ├── SlotType.cs               # NEW: Equipment slot enum (warm-up example)
    │   │   ├── Container.cs              # From W12
    │   │   ├── Inventory.cs              # From W12
    │   │   ├── Equipment.cs              # Extended: CanEquip() slot rule (warm-up)
    │   │   ├── Chest.cs                  # NEW: Container + ILockable
    │   │   ├── MonsterLoot.cs            # NEW: Container
    │   │   ├── Item.cs                   # Extended: EligibleSlot NotMapped property
    │   │   ├── Weapon.cs                 # Extended: EligibleSlot => SlotType.Weapon
    │   │   ├── Armor.cs                  # Extended: EligibleSlot parses Slot string
    │   │   ├── Consumable.cs             # From W12
    │   │   └── KeyItem.cs                # From W12
    │   └── Abilities/
    ├── Helpers/
    │   ├── ConfigurationHelper.cs
    │   └── MigrationHelper.cs
    └── Migrations/
        ├── BaseMigration.cs
        ├── 20260410182937_InitialCreate.cs
        ├── 20260410183100_SeedInitialData.cs
        ├── 20260410192228_AddChestsAndMonsterLoot.cs    # NEW schema migration
        ├── 20260410192408_SeedWorldContent.cs           # NEW seed data migration
        └── Scripts/
            ├── SeedInitialData.sql              # From W12
            ├── SeedInitialData.rollback.sql     # From W12
            ├── SeedWorldContent.sql             # NEW (chests + monster loot)
            └── SeedWorldContent.rollback.sql
```

---

## Assignment Tasks

### Task 1: Run the New Migrations

From the solution directory:

```bash
dotnet ef database update --project ConsoleRpgEntities --startup-project ConsoleRpg
```

This applies **two new migrations** on top of your W12 database:

1. **`AddChestsAndMonsterLoot`** — adds new columns to the `Containers` table (Description, IsLocked, IsTrapped, IsPickable, RequiredKeyId, TrapDamage, TrapDisarmed) and new columns to `Monsters` (LootId, IsLooted). Note: no existing columns change — that's the TPH additive pattern.
2. **`SeedWorldContent`** — runs `Migrations/Scripts/SeedWorldContent.sql` to place three chests in the world and attach a loot container to Grubnak the goblin.

> **Tip:** after running, open SQL Server Object Explorer and SELECT * FROM Containers. You'll see Inventory/Equipment rows from W12 mixed with Chest/MonsterLoot rows. All in one table. That's TPH.

### Task 2: Explore the Seeded Chests

The seed script creates four chests with very different states:

| Id | Description | IsLocked | IsTrapped | Pick? | Required Key | Contents |
|----|-------------|----------|-----------|-------|--------------|----------|
| 3 | Weathered wooden chest | no | no | yes | — | Potion, rusty dagger |
| 4 | Iron-banded chest | yes | no | yes | — | Silvered shortsword, bracers |
| 5 | Ornate rune-engraved chest | yes | no | **no** | `dungeon-main` | Ember wand, mithril chainmail, elixir |
| 6 | Dusty humming chest | no | **yes** | — | — | Trapmaker's dagger, antidote |

And Grubnak the Goblin drops:
- Goblin Cleaver (weapon)
- **Dungeon Key** (KeyItem with `KeyId = "dungeon-main"`) — needed to open chest Id 5!
- Gobbo's Stew (consumable)

See the arc? You fight the goblin, loot it, get the key, then unlock the ornate chest. That's a progression loop built from pure data.

### Task 3: Read `Player.OpenChest`

Open `Models/Characters/Player.cs` and find `OpenChest`. Notice:

- It returns an **`enum`** (`OpenResult`) rather than a bool. This is a common pattern when "did it work?" has more than two outcomes.
- The trap logic fires **once** and sets `TrapDisarmed = true` so it doesn't fire again.
- The `Locked` case doesn't print anything — the caller (in `GameEngine.cs`) is responsible for prompting the player to pick the lock or use a key.

**Discussion prompt:** why do you think `OpenChest` doesn't do the unlock logic itself? Why is it split across `OpenChest` and `TryUnlock`?

*(Hint: single responsibility. Opening a chest and unlocking a chest are two different actions, and the player might try to open the same chest multiple times before finding a key.)*

### Task 4: Read `Player.TryUnlock`

Notice the two branches:
- **Lockpick** (KeyItem where `KeyId == null`) — works only if the chest is `IsPickable` AND has no `RequiredKeyId`. The lockpick is consumed on use.
- **Specific Key** (KeyItem where `KeyId != null`) — works only if it matches `chest.RequiredKeyId`. The key is NOT consumed (you might need it again).

### Task 5: Your Assignment — Add Two New Chest Features

Your job is to extend the chest system with two new features using LINQ:

**A. "Show me the most valuable unopened chest"**
Write a method in `GameEngine.cs` that:
- Uses `_chests.Where(c => c.IsLocked)` to get locked chests
- For each, sums the total Value of items inside (`c.Items.Sum(i => i.Value)`)
- Uses `OrderByDescending` to find the richest one
- Prints the chest's description and total item value
- Wire it into the Chest menu as a new option

**B. "Disarm a trap with a lockpick"**
Add a method to `Player.cs`:

```csharp
public bool DisarmTrap(Chest chest, KeyItem lockpick)
{
    // Only lockpicks (KeyId == null) can disarm traps
    // Only trapped chests can be disarmed
    // On success: set TrapDisarmed = true, remove the lockpick from inventory
    // Return true on success, false otherwise
}
```

Then add it to the chest interaction flow: after attempting to open a chest and finding it trapped, if the player has a lockpick, offer to disarm instead of triggering the trap.

---

## LINQ Patterns You'll Use This Week

```csharp
// Get all chests from the Containers DbSet using TPH
var chests = _context.Containers.OfType<Chest>().ToList();

// Filter chests by state
var lockedChests = chests.Where(c => c.IsLocked);
var trappedChests = chests.Where(c => c.IsTrapped && !c.TrapDisarmed);

// Sum total value of items in a chest
int totalValue = chest.Items.Sum(i => i.Value);

// Find the richest locked chest
var richest = chests
    .Where(c => c.IsLocked)
    .OrderByDescending(c => c.Items.Sum(i => i.Value))
    .FirstOrDefault();

// Get all lockpicks (KeyItems with no specific KeyId) from inventory
var lockpicks = player.Inventory.Items
    .OfType<KeyItem>()
    .Where(k => k.KeyId == null)
    .ToList();

// Group monsters by whether they've been looted
var lootGroups = _context.Monsters
    .GroupBy(m => m.IsLooted)
    .Select(g => new { Looted = g.Key, Count = g.Count() });
```

---

## Stretch Goal (+10%)

**Monster Drop Tables**

Right now Grubnak's loot is hardcoded into the seed script. For this stretch, build a **drop table system**:

1. Create a new entity called `DropTable`:
   ```csharp
   public class DropTable
   {
       public int Id { get; set; }
       public string MonsterType { get; set; } = string.Empty; // "Goblin", "Troll", etc.
       public int ItemId { get; set; }
       public virtual Item Item { get; set; } = null!;
       public int DropChance { get; set; }  // percentage 0-100
   }
   ```

2. Add a `DbSet<DropTable>` to `GameContext`.

3. Generate a migration and seed a few drop entries (e.g., Goblin has a 75% chance to drop "Goblin Cleaver", 25% to drop "Dungeon Key").

4. Write a `RollLoot(Monster monster)` method on Player that, on monster defeat:
   - Queries `_context.DropTable.Where(d => d.MonsterType == monster.MonsterType).ToList()` using LINQ
   - For each row, rolls a random number and if it's below `DropChance`, creates a new Item instance and adds it to the monster's `MonsterLoot` container
   - Notice: each roll creates a **new Item instance** — this is the W12 "items are instances, not types" principle in action

5. Bonus: use `.GroupBy(d => d.MonsterType).Select(g => new { Type = g.Key, Drops = g.Count() })` to print a summary of how many drops each monster type has.

---

## Grading Rubric

| Criteria | Points | Description |
|----------|--------|-------------|
| Migrations Run Cleanly | 15 | Both new migrations apply with no errors |
| Understands the OCP Pattern | 15 | Can explain why adding Chest/MonsterLoot didn't require modifying existing code |
| Understands `ILockable` Separation | 10 | Can explain why chests use two interfaces instead of one |
| Task A: Richest Chest LINQ | 25 | `Where` + nested `Sum` + `OrderByDescending` |
| Task B: DisarmTrap Method | 25 | Correctly consumes lockpick, sets TrapDisarmed, integrates with flow |
| Code Quality | 10 | Clean, readable, follows existing patterns |
| **Total** | **100** | |
| **Stretch: Drop Tables** | **+10** | DropTable entity + random loot generation |

---

## How This Connects to Future Weeks

| Week | What gets added | What carries over |
|------|-----------------|-------------------|
| **W14** | `Room` as yet another Container subclass, plus `Door` as an `ILockable` entity | Your `TryUnlock` logic applies directly to doors — same interface, different target |
| **W15 (final)** | Full world navigation, ASCII map, expanded combat and inventory | All the patterns you've built so far compose into the finished game |

**Key insight:** `ILockable` is the first interface in this course that will be implemented by TWO completely different entities (Chest this week, Door next week). When you see your W13 unlock code working on W14 doors without modification, that's the SOLID principles clicking into place.

---

## Tips

- **`OfType<T>()`** is your friend for TPH queries. `_context.Containers.OfType<Chest>()` filters by discriminator AND casts in one step.
- **Enums for return values** (like `OpenResult`) are cleaner than bool when there are more than two outcomes. Use a `switch` expression to handle each case.
- Remember to call `_context.SaveChanges()` after any state change you want persisted (chest unlocked, trap disarmed, item moved).
- When testing, open SQL Server Object Explorer and watch the `Containers` and `Items` tables update in real time as you play.
- If you break the seed data while testing, run `dotnet ef database update 0 --project ConsoleRpgEntities --startup-project ConsoleRpg` to wipe everything, then `database update` again to reseed.

---

## Submission

1. Commit your changes with a meaningful message
2. Push to your GitHub Classroom repository
3. Submit the repository URL in Canvas

---

## Resources

- [EF Core TPH Inheritance](https://learn.microsoft.com/en-us/ef/core/modeling/inheritance)
- [Enumerable.OfType<TResult>](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.oftype)
- [C# enum with switch expression](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression)
- [Interface Segregation Principle](https://en.wikipedia.org/wiki/Interface_segregation_principle)

---

## Need Help?

- Post questions in the Canvas discussion board
- Attend office hours
- Review the in-class repository for additional examples
