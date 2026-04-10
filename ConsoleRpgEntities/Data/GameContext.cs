using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Containers;
using Microsoft.EntityFrameworkCore;

namespace ConsoleRpgEntities.Data;

/// <summary>
/// GameContext - EF Core database context for the ConsoleRPG game.
///
/// Week 12 introduced two TPH hierarchies:
///   1. Container  → Inventory, Equipment
///   2. Item       → Weapon, Armor, Consumable, KeyItem
///
/// Week 13 extends the Container hierarchy with TWO new subclasses:
///   - Chest       - placeable containers that can be locked/trapped/picked
///   - MonsterLoot - containers attached to monsters, looted on defeat
///
/// Notice how extending the system required ZERO changes to Items, Inventory,
/// or Equipment. That's the Open/Closed Principle - the model is open to
/// extension (new Container types) but closed to modification (existing types
/// don't change). This is why the TPH + interface pattern from W12 was set up
/// the way it was.
/// </summary>
public class GameContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Monster> Monsters { get; set; }
    public DbSet<Ability> Abilities { get; set; }

    // New in Week 12
    public DbSet<Container> Containers { get; set; }
    public DbSet<Item> Items { get; set; }

    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ============================================
        // TPH: Monster hierarchy (from Week 10)
        // ============================================
        modelBuilder.Entity<Monster>()
            .HasDiscriminator<string>(m => m.MonsterType)
            .HasValue<Goblin>("Goblin");

        // ============================================
        // TPH: Ability hierarchy (from Week 10)
        // ============================================
        modelBuilder.Entity<Ability>()
            .HasDiscriminator<string>(a => a.AbilityType)
            .HasValue<ShoveAbility>("ShoveAbility");

        // Many-to-many: Player <-> Ability
        modelBuilder.Entity<Player>()
            .HasMany(p => p.Abilities)
            .WithMany(a => a.Players)
            .UsingEntity(j => j.ToTable("PlayerAbilities"));

        // ============================================
        // TPH: Container hierarchy (extended in Week 13)
        // ============================================
        // All containers live in ONE "Containers" table with a ContainerType
        // discriminator. Week 13 adds Chest and MonsterLoot - just two lines!
        modelBuilder.Entity<Container>()
            .HasDiscriminator<string>(c => c.ContainerType)
            .HasValue<Inventory>("Inventory")
            .HasValue<Equipment>("Equipment")
            .HasValue<Chest>("Chest")
            .HasValue<MonsterLoot>("MonsterLoot");

        // ============================================
        // TPH: Item hierarchy (NEW in Week 12)
        // ============================================
        // All items (Weapons, Armor, Consumables, KeyItems) live in ONE "Items"
        // table with an ItemType discriminator.
        modelBuilder.Entity<Item>()
            .HasDiscriminator<string>(i => i.ItemType)
            .HasValue<Weapon>("Weapon")
            .HasValue<Armor>("Armor")
            .HasValue<Consumable>("Consumable")
            .HasValue<KeyItem>("KeyItem");

        // ============================================
        // Container <-> Item relationship (one-to-many)
        // ============================================
        // Each item has exactly ONE current container. Each container has many items.
        // This is NOT many-to-many - an item can't be in two containers at once.
        // See the README for a discussion of why items are instances, not types.
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Container)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.ContainerId)
            .OnDelete(DeleteBehavior.SetNull);

        // ============================================
        // Player -> Inventory / Equipment (one-way)
        // ============================================
        // Player holds the FKs; Inventory and Equipment don't back-reference the Player.
        // This avoids a duplicate PlayerId column in the TPH Containers table.
        modelBuilder.Entity<Player>()
            .HasOne(p => p.Inventory)
            .WithMany()
            .HasForeignKey(p => p.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Player>()
            .HasOne(p => p.Equipment)
            .WithMany()
            .HasForeignKey(p => p.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================================
        // Monster -> MonsterLoot (NEW in Week 13)
        // ============================================
        // Each monster has its own loot container. The FK lives on Monster.
        modelBuilder.Entity<Monster>()
            .HasOne(m => m.Loot)
            .WithMany()
            .HasForeignKey(m => m.LootId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
