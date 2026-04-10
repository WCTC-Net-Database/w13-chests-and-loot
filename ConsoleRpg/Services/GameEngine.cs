using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Containers;
using Microsoft.EntityFrameworkCore;

namespace ConsoleRpg.Services;

/// <summary>
/// GameEngine - Week 13 entry point for chests, monster loot, and combat.
///
/// Builds on the Week 12 inventory menu by adding:
///   - A chest menu (list chests, attempt to open/unlock/pick/loot)
///   - A combat flow that offers to loot defeated monsters
///
/// The data loading is the important part here: the whole world graph
/// (player, inventory, equipment, monsters, monster loot, chests) is eager-
/// loaded in SetupGame() so the menu code can work without additional
/// database round trips.
/// </summary>
public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;

    private Player? _player;
    private List<Monster> _monsters = new();
    private List<Chest> _chests = new();

    public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager)
    {
        _menuManager = menuManager;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (!_menuManager.ShowMainMenu())
            return;

        SetupGame();
        if (_player == null)
        {
            _outputManager.WriteLine("No player found in the database. Run the seed migration first.", ConsoleColor.Red);
            _outputManager.Display();
            return;
        }

        GameLoop();
    }

    private void SetupGame()
    {
        _player = _context.Players
            .Include(p => p.Inventory!)
                .ThenInclude(i => i.Items)
            .Include(p => p.Equipment!)
                .ThenInclude(e => e.Items)
            .Include(p => p.Abilities)
            .FirstOrDefault();

        _monsters = _context.Monsters
            .Include(m => m.Loot!)
                .ThenInclude(l => l.Items)
            .ToList();

        // Chests are any Container row with ContainerType = "Chest". We query the
        // base Container DbSet and filter with OfType<Chest>() so EF Core loads only
        // those discriminator rows.
        _chests = _context.Containers
            .OfType<Chest>()
            .Include(c => c.Items)
            .ToList();

        if (_player != null)
        {
            _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);
            _outputManager.Display();
            Thread.Sleep(400);
        }
    }

    private void GameLoop()
    {
        while (true)
        {
            _outputManager.Clear();
            _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Inventory Management");
            _outputManager.WriteLine("2. Chests");
            _outputManager.WriteLine("3. Combat");
            _outputManager.WriteLine("4. Quit");
            _outputManager.Display();

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    InventoryMenu();
                    break;
                case "2":
                    ChestMenu();
                    break;
                case "3":
                    CombatMenu();
                    break;
                case "4":
                    return;
                default:
                    _outputManager.WriteLine("Invalid selection.", ConsoleColor.Red);
                    _outputManager.Display();
                    Thread.Sleep(600);
                    break;
            }
        }
    }

    // ============================================================
    // INVENTORY MENU (from Week 12)
    // ============================================================
    private void InventoryMenu()
    {
        if (_player?.Inventory == null)
            return;

        while (true)
        {
            _outputManager.Clear();
            _outputManager.WriteLine("Inventory Management", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. List all items");
            _outputManager.WriteLine("2. Search by name");
            _outputManager.WriteLine("3. Group by type");
            _outputManager.WriteLine("4. Sort items");
            _outputManager.WriteLine("5. Equip item");
            _outputManager.WriteLine("6. Use consumable");
            _outputManager.WriteLine("7. Drop item");
            _outputManager.WriteLine("0. Back");
            _outputManager.Display();

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": ListItems(); break;
                case "2": SearchByName(); break;
                case "3": GroupByType(); break;
                case "4": SortSubmenu(); break;
                case "5": EquipItem(); break;
                case "6": UseConsumable(); break;
                case "7": DropItem(); break;
                case "0": return;
                default:
                    _outputManager.WriteLine("Invalid selection.", ConsoleColor.Red);
                    _outputManager.Display();
                    Thread.Sleep(600);
                    break;
            }
        }
    }

    private void ListItems()
    {
        if (_player?.Inventory == null) return;

        Console.WriteLine($"\n{_player.Name}'s backpack ({_player.GetCurrentWeight()}/{_player.Inventory.MaxWeight} lbs):");
        if (!_player.Inventory.Items.Any())
        {
            Console.WriteLine("  (empty)");
        }
        else
        {
            foreach (var item in _player.Inventory.Items)
            {
                Console.WriteLine($"  - {item.Name} [{item.ItemType}] ({item.Weight} lbs, {item.Value}g)");
            }
        }
        Pause();
    }

    private void SearchByName()
    {
        if (_player?.Inventory == null) return;

        Console.Write("\nEnter search term: ");
        var term = Console.ReadLine() ?? string.Empty;

        var results = _player.Inventory.Items
            .Where(i => i.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (results.Any())
        {
            Console.WriteLine($"\nFound {results.Count} item(s):");
            foreach (var item in results)
                Console.WriteLine($"  - {item.Name} [{item.ItemType}]");
        }
        else
        {
            Console.WriteLine("No matching items.");
        }
        Pause();
    }

    private void GroupByType()
    {
        if (_player?.Inventory == null) return;

        var groups = _player.Inventory.Items
            .GroupBy(i => i.ItemType)
            .OrderBy(g => g.Key);

        Console.WriteLine();
        foreach (var group in groups)
        {
            Console.WriteLine($"{group.Key} ({group.Count()}):");
            foreach (var item in group)
                Console.WriteLine($"  - {item.Name}");
        }
        Pause();
    }

    private void SortSubmenu()
    {
        if (_player?.Inventory == null) return;

        Console.WriteLine("\nSort by:");
        Console.WriteLine("1. Name");
        Console.WriteLine("2. Weight (heaviest first)");
        Console.WriteLine("3. Value (most valuable first)");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        var sorted = choice switch
        {
            "1" => _player.Inventory.Items.OrderBy(i => i.Name).ToList(),
            "2" => _player.Inventory.Items.OrderByDescending(i => i.Weight).ToList(),
            "3" => _player.Inventory.Items.OrderByDescending(i => i.Value).ToList(),
            _ => _player.Inventory.Items.ToList()
        };

        Console.WriteLine();
        foreach (var item in sorted)
            Console.WriteLine($"  {item.Name,-30} {item.Weight,5} lbs  {item.Value,5}g");
        Pause();
    }

    private void EquipItem()
    {
        if (_player?.Inventory == null) return;

        var equippable = _player.Inventory.Items
            .Where(i => i is Weapon || i is Armor)
            .ToList();

        if (!equippable.Any())
        {
            Console.WriteLine("Nothing equippable in your backpack.");
            Pause();
            return;
        }

        Console.WriteLine();
        for (int i = 0; i < equippable.Count; i++)
            Console.WriteLine($"  {i + 1}. {equippable[i].Name}");

        Console.Write("Which item? ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= equippable.Count)
        {
            _player.Equip(equippable[idx - 1]);
            _context.SaveChanges();
        }
        Pause();
    }

    private void UseConsumable()
    {
        if (_player?.Inventory == null) return;

        var consumables = _player.Inventory.Items.OfType<Consumable>().ToList();
        if (!consumables.Any())
        {
            Console.WriteLine("No consumables in your backpack.");
            Pause();
            return;
        }

        Console.WriteLine();
        for (int i = 0; i < consumables.Count; i++)
            Console.WriteLine($"  {i + 1}. {consumables[i].Name} ({consumables[i].EffectType} {consumables[i].EffectAmount})");

        Console.Write("Which item? ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= consumables.Count)
        {
            _player.UseItem(consumables[idx - 1]);
            _context.SaveChanges();
        }
        Pause();
    }

    private void DropItem()
    {
        if (_player?.Inventory == null) return;

        var items = _player.Inventory.Items.ToList();
        if (!items.Any())
        {
            Console.WriteLine("Backpack is empty.");
            Pause();
            return;
        }

        Console.WriteLine();
        for (int i = 0; i < items.Count; i++)
            Console.WriteLine($"  {i + 1}. {items[i].Name}");

        Console.Write("Which item? ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= items.Count)
        {
            var item = items[idx - 1];
            _player.Drop(item);
            // In W13 dropped items are still orphaned (ContainerId = null).
            // In W14 we'll place them into the current Room's container instead.
            _context.Items.Remove(item);
            _context.SaveChanges();
        }
        Pause();
    }

    // ============================================================
    // CHEST MENU (new in Week 13)
    // ============================================================
    private void ChestMenu()
    {
        if (_player == null) return;

        while (true)
        {
            _outputManager.Clear();
            _outputManager.WriteLine("Chests", ConsoleColor.Cyan);

            if (!_chests.Any())
            {
                Console.WriteLine("\nNo chests in the world.");
                Pause();
                return;
            }

            // List each chest with a quick status summary
            for (int i = 0; i < _chests.Count; i++)
            {
                var chest = _chests[i];
                var status = chest.IsLocked ? "[LOCKED]" : chest.Items.Any() ? "[OPEN]" : "[EMPTY]";
                var trap = chest.IsTrapped && !chest.TrapDisarmed ? " [TRAPPED]" : "";
                Console.WriteLine($"  {i + 1}. {chest.Description} {status}{trap}");
            }
            Console.WriteLine("  0. Back");
            Console.Write("\nWhich chest? ");

            var input = Console.ReadLine();
            if (input == "0") return;

            if (!int.TryParse(input, out int idx) || idx < 1 || idx > _chests.Count)
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                continue;
            }

            InteractWithChest(_chests[idx - 1]);
        }
    }

    private void InteractWithChest(Chest chest)
    {
        if (_player == null) return;

        // Attempt to open
        var result = _player.OpenChest(chest);

        switch (result)
        {
            case Player.OpenResult.Locked:
                Console.WriteLine("\nThe chest is locked.");
                TryUnlockPrompt(chest);
                break;

            case Player.OpenResult.Trapped:
                // Trap already fired inside OpenChest(). Save player damage and continue.
                _context.SaveChanges();
                LootChestPrompt(chest);
                break;

            case Player.OpenResult.Opened:
                LootChestPrompt(chest);
                break;

            case Player.OpenResult.AlreadyOpen:
                LootChestPrompt(chest);
                break;
        }
    }

    private void TryUnlockPrompt(Chest chest)
    {
        if (_player?.Inventory == null) return;

        var keys = _player.Inventory.Items.OfType<KeyItem>().ToList();
        if (!keys.Any())
        {
            Console.WriteLine("You have no keys or lockpicks.");
            Pause();
            return;
        }

        Console.WriteLine("\nChoose a key to try:");
        for (int i = 0; i < keys.Count; i++)
        {
            var label = keys[i].KeyId == null ? "(lockpick)" : $"(key: {keys[i].KeyId})";
            Console.WriteLine($"  {i + 1}. {keys[i].Name} {label}");
        }
        Console.Write("Which one? ");

        if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= keys.Count)
        {
            if (_player.TryUnlock(chest, keys[idx - 1]))
            {
                _context.SaveChanges();

                // Attempt to open again, now that it's unlocked
                var second = _player.OpenChest(chest);
                if (second == Player.OpenResult.Trapped)
                    _context.SaveChanges();
                LootChestPrompt(chest);
            }
        }
        Pause();
    }

    private void LootChestPrompt(Chest chest)
    {
        if (_player == null) return;

        if (!chest.Items.Any())
        {
            Console.WriteLine("\nThe chest is empty.");
            Pause();
            return;
        }

        Console.WriteLine("\nContents:");
        foreach (var item in chest.Items)
            Console.WriteLine($"  - {item.Name} [{item.ItemType}]");

        Console.Write("\nTake all? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            _player.LootChest(chest);
            _context.SaveChanges();
        }
        Pause();
    }

    // ============================================================
    // COMBAT MENU (updated in Week 13 for monster looting)
    // ============================================================
    private void CombatMenu()
    {
        if (_player == null) return;

        var alive = _monsters.Where(m => m.Health > 0).ToList();
        if (!alive.Any())
        {
            Console.WriteLine("\nNo monsters left to fight.");
            Pause();
            return;
        }

        Console.WriteLine("\nPick a target:");
        for (int i = 0; i < alive.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {alive[i].Name} (HP: {alive[i].Health})");
        }
        Console.Write("Which? ");

        if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > alive.Count)
            return;

        var target = alive[idx - 1];
        _player.Attack(target);

        // Monster counterattacks if still alive
        if (target.Health > 0)
        {
            target.Attack(_player);
        }
        else
        {
            Console.WriteLine($"{target.Name} has been defeated!");
            _context.SaveChanges();

            // Offer to loot
            Console.Write("\nLoot the corpse? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() == "y")
            {
                _player.LootMonster(target);
                _context.SaveChanges();
            }
        }

        Pause();
    }

    private void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
