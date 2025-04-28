
public static class Utils
{
    // General constants
    public const string gamePrefix = "game_";

    // Server constants
    public const string identityPoolId = "us-east-2:602a8446-6ff3-4035-a407-b643051e82bc";
    public const string lambdaFunction = "MultiplayerFunction";

    // Game constants
    public const float timeOutAfter = 20f;
    public const float sendPlayerDataInterval = 1f;
    public const float runSpeed = 7f;
    public const float walkBack = -2f;
    public const float rotateSpeed = 50f;
    public const int hitLifeValue = 10;

    // Player constants
    public const int statusNoMove = 0;
    public const int statusMoveRun = 10;
    public const int statusMoveBack = 11;

    public const int statusNoRotate = 0;
    public const int statusRotateLeft = 1;
    public const int statusRotateRight = 2;

    public const int statusNoAction = 0;
    public const int statusActionRun = 1;
    public const int statusActionWalkBack = 2;
    public const int statusActionPunch = 10;
    public const int statusActionFlyingPunch = 11;
    public const int statusActionHitToHead = 20;
    public const int statusActionDie = 30;
}
