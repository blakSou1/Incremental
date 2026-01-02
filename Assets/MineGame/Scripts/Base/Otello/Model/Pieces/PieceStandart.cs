using System.Collections.Generic;
using System.Linq;

public class PieceStandart : Piece
{
    public override bool CheckPieceValid(GridBox.Status color, GridBox curentBox, out List<GridBox> revColorPieces)
    {
        revColorPieces = new List<GridBox>();

        if (curentBox?.GetStat() != GridBox.Status.None)
            return false;

        List<GridBox> pieceList = G.gridFuncion.GetCrossPieces(curentBox.GetIndex());
        List<GridBox> sameColorPieces = pieceList.Where(piece => piece.GetStat() == color).ToList();
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (G.gridFuncion.IsAdjacent(curentBox.GetIndex(), piece.GetIndex())) continue;

            List<GridBox> crossList = G.gridFuncion.GetCrossPieces(curentBox.GetIndex(), piece.GetIndex(), false);
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
                G.gridFuncion.blackPieces.Add(revColorPiece.piece);
                G.gridFuncion.whitePieces.Remove(revColorPiece.piece);
            }
            else
            {
                G.gridFuncion.whitePieces.Add(revColorPiece.piece);
                G.gridFuncion.blackPieces.Remove(revColorPiece.piece);
            }
        }
    }
}
