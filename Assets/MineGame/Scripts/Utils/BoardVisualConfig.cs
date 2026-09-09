using UnityEngine;


[CreateAssetMenu(fileName = "BoardVisualConfig ", menuName = "Board/Visual Config")]
public class BoardVisualConfig : ScriptableObject
{
    [Header("Ячейки")]
    [Tooltip("Префаб ячейки игрового поля")]
    public GridBox cellPrefab;

    [Header("Отступы")]
    [Tooltip("Расстояние между ячейками")]
    public Vector2 cellSpacing;

    [Header("Индикация")]
    [Tooltip("Объект для подсветки выбранной ячейки")]
    public Indic highlightIndicator;

}
