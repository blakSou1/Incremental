using System.Collections.Generic;

public class PieceShield : PieceBase
{
    public PieceShield()
    {
        id = "PieceSheild";
        idS = id;
        Description = new("1 time prevents a coup");

        Define<TagPieceRule>().valid = ReversiPieceRule.Instance;
        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }

    public override void FlipOfPiece(List<GridBox> revColorPieces)
    {
        foreach (var revColorPiece in revColorPieces)
            revColorPiece.Flip();
    }
}
