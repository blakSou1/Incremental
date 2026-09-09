using System.Collections.Generic;
using System.Linq;

public class BasicPiece : PieceBase
{
    public BasicPiece()
    {
        id = ConfigGame.standardPiece;
        idS = id;

        Define<TagPieceRule>().valid = ReversiPieceRule.Instance;
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

public abstract class PieceBase : CMSEntity
{
    public static string idS;
    public string Description;

    public PieceBase()
    {
        Define<TagPrefab>().prefab = "prefab/Piece/name".Load<InteractiveObject>();
        Define<TagRarity>().rarity = PieceRarity.COMMON;
        Define<TagPieceRule>().valid = PieceRule.Instance;

        Define<TagExcludeFromReward>();
        id = "name";
        idS = id;
    }

    public virtual void FlipOfPiece(List<GridBox> sameColorPieces)
    {
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

public class ReversiPieceRule : PieceRule
{
    public static readonly ReversiPieceRule Instance = new();

    public override bool IsMoveValid(Status color, GridBox currentBox, out List<GridBox> piecesToFlip)
    {
        piecesToFlip = new List<GridBox>();

        if (currentBox?.GetStat() != Status.None)
            return false;

        List<GridBox> linePieces = G.mainEnterPoint.gridController.GetCrossPieces(currentBox.GetIndex());
        List<GridBox> allyPieces = linePieces.Where(piece => piece.GetStat() == color).ToList();
        var canPlace = false;

        foreach (var piece in allyPieces)
        {
            if (G.mainEnterPoint.gridController.IsAdjacent(currentBox.GetIndex(), piece.GetIndex())) continue;

            List<GridBox> lineBoxes = G.mainEnterPoint.gridController.GetCrossPieces(currentBox.GetIndex(), piece.GetIndex(), false);
            var flippablePieces = lineBoxes.Where(p => p.GetStat() == (Status)((int)color * -1)).ToList();
            bool isLineBlocked = lineBoxes.Any(p => p.GetStat() == Status.None || p.GetStat() == color);

            if (flippablePieces.Count() != 0 && !isLineBlocked)
            {
                canPlace = true;
                piecesToFlip.AddRange(flippablePieces);
            }
        }

        return canPlace;
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

        return true;
    }
}

public class PieceRule
{
    public readonly static PieceRule Instance = new();

    public virtual bool IsMoveValid(Status color, GridBox currentBox, out List<GridBox> piecesToFlip)
    {
        piecesToFlip = null;
        return true;
    }
}

#endregion