using System.Collections.Generic;
using System.Linq;

public class PieceSheild : PieceBase
{
    public PieceSheild()
    {
        id = "PieceSheild";
        idS = id;
        Description = new(en: "1 time prevents a coup", ru: "1 Раз препятствует перевороту");

        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }


    public override bool CheckPieceValid(Status color, GridBox curentBox, out List<GridBox> revColorPieces)
    {
        revColorPieces = new List<GridBox>();

        if (curentBox?.GetStat() != Status.None)
            return false;

        List<GridBox> pieceList = G.gridController.GetCrossPieces(curentBox.GetIndex());
        List<GridBox> sameColorPieces = pieceList.Where(piece => piece.GetStat() == color).ToList();
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (G.gridController.IsAdjacent(curentBox.GetIndex(), piece.GetIndex())) continue;

            List<GridBox> crossList = G.gridController.GetCrossPieces(curentBox.GetIndex(), piece.GetIndex(), false);
            var enemyPiecesInLine = crossList.Where(p => p.GetStat() == (Status)((int)color * -1)).ToList();
            bool lineIsBlocked = crossList.Any(p => p.GetStat() == Status.None || p.GetStat() == color);

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
        }
    }
}
