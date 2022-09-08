public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum AimDirection
{
    Up,
    UpLeft,
    UpRight,
    Right,
    Left,
    Down
}

public enum ChestSpawnEvent
{
    onRoomEntry,
    onEnemiesDefeated
}

public enum ChestSpawnPosition
{
    atSpawnerPosition,
    atPlayerPosition
}

public enum ChestState
{
    closed,
    healthItem,
    ammoItem,
    weaponItem,
    empty
}

public enum GameState
{
    gameStarted,
    playingLevel,
    killingEnemies,
    BossStage,
    killingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused, 
    dungeonOverviewMap,
    gameRestarted
}

