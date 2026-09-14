
public class PieceFullMove : PieceBase
{
    public PieceFullMove()
    {
        id = "PieceMoveFull";
        Description = new("Moves to any square on the field");

        Define<TagPieceRule>().valid = FreePlacementRule.Instance;
        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }
}
