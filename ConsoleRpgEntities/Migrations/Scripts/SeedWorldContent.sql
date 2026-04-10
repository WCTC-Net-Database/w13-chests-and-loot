-- ================================================================
-- Week 13 Seed Data: World Content
-- ================================================================
-- Builds on the W12 SeedInitialData script by adding:
--   - 3 chests scattered around the world (various states)
--   - 1 monster loot container linked to the existing Goblin
--   - New items inside each of the new containers
--   - A few extra KeyItems in the player's inventory (including a specific key)
--
-- Assumptions (from W12 SeedInitialData):
--   - Container Id 1 = Player Inventory ("Elara the Bold")
--   - Container Id 2 = Player Equipment
--   - Player Id 1 = Elara
--   - Monster Id 1 = Grubnak the Greedy (Goblin)
-- ================================================================

-- ----------------------------------------------------------------
-- New Containers: chests and a MonsterLoot
-- ----------------------------------------------------------------
SET IDENTITY_INSERT Containers ON;
INSERT INTO Containers
    (Id, ContainerType, MaxWeight, Description, IsLocked, IsTrapped, IsPickable, RequiredKeyId, TrapDamage, TrapDisarmed)
VALUES
    -- Id 3: Simple wooden chest, not locked
    (3, 'Chest', NULL, 'A weathered wooden chest', 0, 0, 1, NULL, 0, 0),

    -- Id 4: Locked chest, pickable (no specific key required)
    (4, 'Chest', NULL, 'An iron-banded chest with a stout lock', 1, 0, 1, NULL, 0, 0),

    -- Id 5: Locked chest, requires the "dungeon-main" key
    (5, 'Chest', NULL, 'An ornate chest engraved with runes', 1, 0, 0, 'dungeon-main', 0, 0),

    -- Id 6: Trapped chest, unlocked
    (6, 'Chest', NULL, 'A dusty old chest humming faintly', 0, 1, 1, NULL, 10, 0),

    -- Id 7: MonsterLoot for Grubnak
    (7, 'MonsterLoot', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
SET IDENTITY_INSERT Containers OFF;

-- ----------------------------------------------------------------
-- Link the MonsterLoot to the goblin
-- ----------------------------------------------------------------
UPDATE Monsters SET LootId = 7 WHERE Id = 1;

-- ----------------------------------------------------------------
-- Chest contents (Container Id 3 - simple wooden chest)
-- ----------------------------------------------------------------
INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, EffectType, EffectAmount, Uses)
VALUES
    ('Minor Healing Potion', 'A small vial of pale red liquid.', 'Consumable', 0.50, 15, 3, 'Heal', 10, 1);

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Attack, Category)
VALUES
    ('Rusted Dagger', 'A dull, pitted blade.', 'Weapon', 1.00, 5, 3, 1, 'Melee');

-- ----------------------------------------------------------------
-- Chest contents (Container Id 4 - locked, pickable iron chest)
-- ----------------------------------------------------------------
INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Attack, Category)
VALUES
    ('Silvered Shortsword', 'A blade etched with silver runes.', 'Weapon', 3.50, 180, 4, 9, 'Melee');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Defense, Slot)
VALUES
    ('Reinforced Bracers', 'Leather bracers with iron plates.', 'Armor', 2.00, 75, 4, 2, 'Hands');

-- ----------------------------------------------------------------
-- Chest contents (Container Id 5 - locked by dungeon-main key)
-- ----------------------------------------------------------------
INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Attack, Category)
VALUES
    ('Ember Wand', 'A slender wand crackling with heat.', 'Weapon', 1.00, 400, 5, 12, 'Magic');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Defense, Slot)
VALUES
    ('Mithril Chainmail', 'Impossibly light yet strong.', 'Armor', 8.00, 650, 5, 8, 'Body');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, EffectType, EffectAmount, Uses)
VALUES
    ('Elixir of Haste', 'Glimmers with quicksilver.', 'Consumable', 0.50, 150, 5, 'Buff', 20, 1);

-- ----------------------------------------------------------------
-- Chest contents (Container Id 6 - trapped, unlocked)
-- ----------------------------------------------------------------
INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Attack, Category)
VALUES
    ('Trapmaker''s Dagger', 'A jagged dagger covered in wire.', 'Weapon', 1.50, 90, 6, 5, 'Melee');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, EffectType, EffectAmount, Uses)
VALUES
    ('Antidote', 'Counteracts poison.', 'Consumable', 0.30, 40, 6, 'Heal', 5, 2);

-- ----------------------------------------------------------------
-- Monster Loot (Container Id 7 - Grubnak's drops)
-- ----------------------------------------------------------------
INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, Attack, Category)
VALUES
    ('Goblin Cleaver', 'A crude but sharp blade.', 'Weapon', 2.00, 25, 7, 3, 'Melee');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, KeyId)
VALUES
    ('Dungeon Key', 'An iron key marked with the dungeon crest.', 'KeyItem', 0.20, 0, 7, 'dungeon-main');

INSERT INTO Items (Name, Description, ItemType, Weight, Value, ContainerId, EffectType, EffectAmount, Uses)
VALUES
    ('Gobbo''s Stew', 'Suspicious brown liquid in a flask.', 'Consumable', 0.50, 5, 7, 'Heal', 5, 1);
