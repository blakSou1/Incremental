
public class PieceShield : PieceBase
{
    public PieceShield()
    {
        id = "PieceSheild";
        Description = new("1 time prevents a coup");

        Define<TagPieceRule>().valid = ReversiPieceRule.Instance;
        Define<TagPrefab>().prefab = ("prefab/Piece/" + $"{id}").Load<InteractiveObject>();
        Define<TagExcludeFromReward>();
    }
}
