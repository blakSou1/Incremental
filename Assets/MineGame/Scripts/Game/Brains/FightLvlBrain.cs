
public class FightLvlBrain : BoardBrain
{
    public override void StartLvl()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.mainEnterPoint.gridController.NewMatrix();
        G.mainEnterPoint.StartCoroutine(StartAnimationSpawnGrid(G.configGame.MatrixModel.matrixField.size, 
            G.mainEnterPoint.gridController.matrix.GetData(), G.mainEnterPoint.gridController.matrix.GetParent()));
    }

}
