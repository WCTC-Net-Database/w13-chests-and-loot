-- Rollback for SeedWorldContent - removes seeded chests, monster loot, and items.

DELETE FROM Items WHERE ContainerId IN (3, 4, 5, 6, 7);

UPDATE Monsters
SET LootId = NULL
WHERE Id = 1;

DELETE FROM Containers WHERE Id IN (3, 4, 5, 6, 7);
