using System.Collections.Generic;
using System.Linq;

public class BasicPiece : PieceBase
{
    public BasicPiece()
    {
        id = ConfigGame.standardPiece;

        Define<TagPieceRule>().valid = ReversiPieceRule.Instance;
        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }
}

public abstract class PieceBase : CMSEntity
{
    public string Description;

    public PieceBase()
    {
        Define<TagPrefab>().prefab = "prefab/Piece/name".Load<InteractiveObject>();
        Define<TagRarity>().rarity = PieceRarity.COMMON;
        Define<TagPieceRule>().valid = PieceRule.Instance;

        Define<TagExcludeFromReward>();
        id = "name";
    }

    public virtual void FlipOfPiece(List<GridBox> sameColorPieces)
    {
        foreach (var revColorPiece in sameColorPieces)
            revColorPiece.Flip();
    }
}

//
//Tags
//

public class TagExcludeFromReward : EntityComponentDefinition
{
}

public enum PieceRarity
{
    COMMON,
    UNCOMMON,
    RARE
}

public class TagRarity : EntityComponentDefinition
{
    public PieceRarity rarity;
}

public class TagPrefab : EntityComponentDefinition
{
    public InteractiveObject prefab;
}

#region TagRule

public class TagPieceRule : EntityComponentDefinition
{
    public PieceRule valid;
}

public class ReversiPieceRule : FreePlacementRule
{
    public static readonly ReversiPieceRule Instance = new();

    protected override bool Returned(List<GridBox> piecesToFlip)
    {
        return piecesToFlip.Count > 0;
    }

}

public class FreePlacementRule : PieceRule
{
    public static readonly FreePlacementRule Instance = new();

    public override bool IsMoveValid(Status color, GridBox currentBox, out List<GridBox> piecesToFlip)
    {
        piecesToFlip = new List<GridBox>();

        if (currentBox?.GetStat() != Status.None)
            return false;

        List<GridBox> pieceList = G.mainEnterPoint.gridController.GetCrossPieces(currentBox.GetIndex());
        List<GridBox> sameColorPieces = pieceList.Where(piece => piece.GetStat() == color).ToList();

        foreach (var piece in sameColorPieces)
        {
            if (G.mainEnterPoint.gridController.IsAdjacent(currentBox.GetIndex(), piece.GetIndex())) continue;

            List<GridBox> crossList = G.mainEnterPoint.gridController.GetCrossPieces(currentBox.GetIndex(), piece.GetIndex(), false);
            var enemyPiecesInLine = crossList.Where(p => p.GetStat() == (Status)((int)color * -1)).ToList();
            bool lineIsBlocked = crossList.Any(p => p.GetStat() == Status.None || p.GetStat() == color);

            if (enemyPiecesInLine.Count() != 0 && !lineIsBlocked)
                piecesToFlip.AddRange(enemyPiecesInLine);
        }

        return Returned(piecesToFlip);
    }

    protected override bool Returned(List<GridBox> piecesToFlip)
    {
        return true;
    }

}

public class PieceRule
{
    public static readonly PieceRule Instance = new();

    public virtual bool IsMoveValid(Status color, GridBox currentBox, out List<GridBox> piecesToFlip)
    {
        piecesToFlip = null;
        return Returned(piecesToFlip);
    }

    protected virtual bool Returned(List<GridBox> piecesToFlip)
    {
        return true;
    }
}

#endregion