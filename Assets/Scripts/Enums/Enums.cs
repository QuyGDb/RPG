public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down,
    DownRight,
    DownLeft,
    None
}

public enum EnemyState
{
    Roaming,
    Chasing,
    Attacking,
    Dead,
    GoBackToStart,
    None
}

public enum BashState
{
    ActiveBash,
    DuringBash,
    ReleaseBash,
    None
}
public enum AmmoState
{
    Trajectory,
    Linear,
    Freeze
}
public enum AnimationEnemyType
{

    Run,
    IdleAndRun,
    IdleRunAndAttack,
}