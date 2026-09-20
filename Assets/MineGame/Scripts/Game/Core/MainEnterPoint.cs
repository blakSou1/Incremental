
public class MainEnterPoint : ManagedBehaviour
{
    public PieceController pieceController;
    public GridController gridController;
    public ModifirePieces modifierPieces;
    public ConditionsOfVictoryAndDefeat conditionsOfVictoryAndDefeat;
    public GameLogic gameLogic;

    BaseBrain brain;

    public void Start()
    {
        InitComponents();

        brain.StartLvl();
    }

    private void InitComponents()
    {
        pieceController = new();
        gridController = new();
        modifierPieces = new();
        conditionsOfVictoryAndDefeat = new();
        gameLogic = new();

        gridController.Init();
        modifierPieces.Init();
        gameLogic.Init();

        G.configGame.GetConfigLevel().matrixNode.AddListenerUp();
        brain = G.configGame.GetConfigLevel().brain;
        brain.Resets();

        G.eventManager.SetEnemyHp.Invoke(G.configGame.GetConfigLevel().enemyConfig.hp);
    }

    protected override void PausableFixedUpdate()
    {
        brain.Tick();
    }

}