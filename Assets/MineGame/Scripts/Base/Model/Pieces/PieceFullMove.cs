using System.Collections.Generic;

public class PieceFullMove : PieceBase
{
    public PieceFullMove()
    {
        id = "PieceMoveFull";
        idS = id;
        Description = new("Moves to any square on the field");

        Define<TagPieceRule>().valid = FreePlacementRule.Instance;
        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }

    public override void FlipOfPiece(List<GridBox> revColorPieces)
    {

        foreach (var revColorPiece in revColorPieces)
        {
            revColorPiece.Flip();
        }
    }
}
