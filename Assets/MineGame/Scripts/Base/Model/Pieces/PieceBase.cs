using System.Collections.Generic;
using System.Linq;

public class BasicPiece : PieceBase
{
    public BasicPiece()
    {
        id = ConfigGame.standertPiece;

        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }

    public override bool CheckPieceValid(GridBox.Status color, GridBox curentBox, out List<GridBox> revColorPieces)
    {
        revColorPieces = new List<GridBox>();

        if (curentBox?.GetStat() != GridBox.Status.None)
            return false;

        List<GridBox> pieceList = G.gridController.GetCrossPieces(curentBox.GetIndex());
        List<GridBox> sameColorPieces = pieceList.Where(piece => piece.GetStat() == color).ToList();
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (G.gridController.IsAdjacent(curentBox.GetIndex(), piece.GetIndex())) continue;

            List<GridBox> crossList = G.gridController.GetCrossPieces(curentBox.GetIndex(), piece.GetIndex(), false);
            var enemyPiecesInLine = crossList.Where(p => p.GetStat() == (GridBox.Status)((int)color * -1)).ToList();
            bool lineIsBlocked = crossList.Any(p => p.GetStat() == GridBox.Status.None || p.GetStat() == color);

            if (enemyPiecesInLine.Count() != 0 && !lineIsBlocked)
            {
                able = true;
                revColorPieces.AddRange(enemyPiecesInLine);
            }
        }

        return able;
    }

    public override void FlipOfPiece(List<GridBox> revColorPieces)
    {

        foreach (var revColorPiece in revColorPieces)
        {
            revColorPiece.Flip();
            if (revColorPiece.GetStat() == GridBox.Status.Black)
            {
                G.gridController.blackPieces.Add(revColorPiece.piece);
                G.gridController.whitePieces.Remove(revColorPiece.piece);
            }
            else
            {
                G.gridController.whitePieces.Add(revColorPiece.piece);
                G.gridController.blackPieces.Remove(revColorPiece.piece);
            }
        }
    }

}

public abstract class PieceBase : CMSEntity
{
    public PieceBase()
    {
        Define<TagPrefab>().prefab = "prefab/Piece/name".Load<InteractiveObject>();
        Define<TagRarity>().rarity = PieceRarity.COMMON;
        Define<TagExcludeFromReward>();
        id = "name";
    }

    public virtual bool CheckPieceValid(GridBox.Status color, GridBox curentBox, out List<GridBox> revColorPieces)
    {
        revColorPieces = null;
        return true;
    }

    public virtual void FlipOfPiece(List<GridBox> sameColorPieces)
    {
    }
}

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