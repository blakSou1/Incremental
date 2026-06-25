using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatrixModel))]
public class MatrixModelEditor : UnityEditor.Editor
{
    private int gridSize = 8;
    private bool showGridSettings = true;
    private Vector2 scrollPosition;

    private Color selectedBorderColor = new(0.2f, 0.5f, 0.9f, 0.8f);

    private Grid selectedGrid = null;
    private Vector2Int selectedPosition = new(-1, -1);

    // Кэш для текстур фишек
    private Texture2D blackPieceTexture;
    private Texture2D whitePieceTexture;
    private Texture2D blackPieceOutlineTexture;
    private Texture2D whitePieceOutlineTexture;

    public override void OnInspectorGUI()
    {
        MatrixModel model = (MatrixModel)target;

        showGridSettings = EditorGUILayout.Foldout(showGridSettings, "Grid Settings");
        if (showGridSettings)
        {
            EditorGUI.indentLevel++;

            int newSize = EditorGUILayout.IntField("Size", gridSize);

            if (newSize != gridSize)
            {
                gridSize = Mathf.Max(1, newSize);
            }

            if (GUILayout.Button("Resize Grid"))
            {
                ResizeGrid(model.matrixField, gridSize);
                EditorUtility.SetDirty(model);
            }

            if (GUILayout.Button("Clear Grid"))
            {
                ClearGrid(model.matrixField);
                EditorUtility.SetDirty(model);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        DrawChessboard(model.matrixField);

        if (selectedGrid != null && selectedPosition.x >= 0 && selectedPosition.y >= 0)
        {
            DrawSelectedGridEditor(selectedGrid, selectedPosition);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click on any cell to select and edit it", MessageType.Info);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(model);
        }
    }

    private void DrawChessboard(MatrixField field)
    {
        if (field.data == null)
        {
            EditorGUILayout.HelpBox("Grid data is null", MessageType.Error);
            return;
        }

        int size = field.size;

        if (size == 0)
        {
            EditorGUILayout.HelpBox("Grid is empty", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Grid: {size}x{size}", EditorStyles.boldLabel);

        float cellSize = Mathf.Min(40f, 600f / Mathf.Max(size, 1));

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Mathf.Min(600, size * (cellSize + 2) + 40)));

        // Заголовки столбцов
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(35);
        for (int x = 0; x < size; x++)
        {
            GUILayout.Label(x.ToString(), GUILayout.Width(cellSize), GUILayout.Height(20));
        }
        EditorGUILayout.EndHorizontal();

        // Строки сетки (снизу вверх)
        for (int y = size - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(y.ToString(), GUILayout.Width(30), GUILayout.Height(cellSize));

            for (int x = 0; x < size; x++)
            {
                Grid grid = field.data.GetValue(x, y);
                if (grid == null)
                {
                    grid = new Grid();
                    field.data.SetValue(x, y, grid);
                }

                bool isSelected = (selectedGrid == grid && selectedPosition.x == x && selectedPosition.y == y);

                // Создаем прямоугольник для ячейки
                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                // Рисуем шахматный фон
                Color cellColor = GetChessboardColor(x, y);
                EditorGUI.DrawRect(rect, cellColor);

                // Рисуем выделение
                if (isSelected)
                {
                    Handles.DrawSolidRectangleWithOutline(rect, Color.clear, selectedBorderColor);
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.6f, 1f, 0.15f));
                }
                else
                {
                    Handles.DrawSolidRectangleWithOutline(rect, Color.clear, new Color(0.5f, 0.5f, 0.5f, 0.2f));
                }

                // Рисуем фишку (кружок) если статус не None
                if (grid.stat != Status.None)
                {
                    DrawPiece(rect, grid.stat);
                }

                // Отображаем текст (ID Piece и Modifier)
                string displayText = GetGridDisplayText(grid);
                if (!string.IsNullOrEmpty(displayText))
                {
                    Color textColor = GetTextColor(cellColor);
                    GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                    textStyle.normal.textColor = textColor;
                    textStyle.alignment = TextAnchor.MiddleCenter;
                    textStyle.fontSize = Mathf.RoundToInt(cellSize / 4);
                    textStyle.fontStyle = FontStyle.Bold;

                    GUI.Label(rect, displayText, textStyle);
                }

                // Обработка клика по ячейке
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    selectedGrid = grid;
                    selectedPosition = new Vector2Int(x, y);
                    Event.current.Use();
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPiece(Rect cellRect, Status status)
    {
        // Определяем размер фишки (70% от ячейки)
        float pieceSize = Mathf.Min(cellRect.width, cellRect.height) * 0.7f;

        // Минимальный размер фишки
        if (pieceSize < 4f) return;

        float offsetX = (cellRect.width - pieceSize) / 2f;
        float offsetY = (cellRect.height - pieceSize) / 2f;

        Rect pieceRect = new(
            cellRect.x + offsetX,
            cellRect.y + offsetY,
            pieceSize,
            pieceSize
        );

        // Получаем или создаем текстуру для фишки
        Texture2D pieceTexture = GetPieceTexture(status);
        Texture2D outlineTexture = GetPieceOutlineTexture(status);

        if (pieceTexture != null && outlineTexture != null)
        {
            // Рисуем ободок
            Rect outlineRect = new Rect(
                pieceRect.x - 1,
                pieceRect.y - 1,
                pieceRect.width + 2,
                pieceRect.height + 2
            );
            GUI.DrawTexture(outlineRect, outlineTexture);

            // Рисуем саму фишку
            GUI.DrawTexture(pieceRect, pieceTexture);
        }
    }

    private Texture2D GetPieceTexture(Status status)
    {
        if (status == Status.Black)
        {
            if (blackPieceTexture == null)
                blackPieceTexture = CreateCircleTexture(64, Color.black);
            return blackPieceTexture;
        }
        else if (status == Status.White)
        {
            if (whitePieceTexture == null)
                whitePieceTexture = CreateCircleTexture(64, Color.white);
            return whitePieceTexture;
        }
        return null;
    }

    private Texture2D GetPieceOutlineTexture(Status status)
    {
        if (status == Status.Black)
        {
            if (blackPieceOutlineTexture == null)
                blackPieceOutlineTexture = CreateCircleTexture(64, new Color(0.7f, 0.7f, 0.7f));
            return blackPieceOutlineTexture;
        }
        else if (status == Status.White)
        {
            if (whitePieceOutlineTexture == null)
                whitePieceOutlineTexture = CreateCircleTexture(64, new Color(0.3f, 0.3f, 0.3f));
            return whitePieceOutlineTexture;
        }
        return null;
    }

    private Texture2D CreateCircleTexture(int size, Color color)
    {
        // Убеждаемся что размер > 0
        size = Mathf.Max(1, size);

        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        float radiusSquared = radius * radius;

        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % size;
            int y = i / size;

            float dx = x - center.x;
            float dy = y - center.y;
            float distanceSquared = dx * dx + dy * dy;

            if (distanceSquared <= radiusSquared)
            {
                // Сглаживание краев
                float distance = Mathf.Sqrt(distanceSquared);
                float alpha = 1f;
                if (distance > radius - 1.5f)
                {
                    alpha = 1f - (distance - (radius - 1.5f)) / 1.5f;
                    alpha = Mathf.Clamp01(alpha);
                }
                colors[i] = new Color(color.r, color.g, color.b, alpha);
            }
            else
            {
                colors[i] = Color.clear;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    private Color GetChessboardColor(int x, int y)
    {
        bool isBlackCell = (x + y) % 2 == 0;
        return isBlackCell ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.7f, 0.7f, 0.7f);
    }

    private Color GetTextColor(Color backgroundColor)
    {
        float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;
        return brightness > 0.5f ? Color.black : Color.white;
    }

    private string GetGridDisplayText(Grid grid)
    {
        string text = "";

        if (!string.IsNullOrEmpty(grid.idPiece))
            text += "P:" + grid.idPiece.Substring(0, Mathf.Min(2, grid.idPiece.Length));

        if (!string.IsNullOrEmpty(grid.idModifireGrid))
        {
            if (!string.IsNullOrEmpty(text))
                text += "\n";
            text += "M:" + grid.idModifireGrid.Substring(0, Mathf.Min(2, grid.idModifireGrid.Length));
        }

        return text;
    }

    private void DrawSelectedGridEditor(Grid grid, Vector2Int position)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Selected Cell: ({position.x}, {position.y})", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        Status oldStatus = grid.stat;
        string oldPiece = grid.idPiece;
        string oldModifier = grid.idModifireGrid;

        grid.stat = (Status)EditorGUILayout.EnumPopup("Status", grid.stat);
        grid.idPiece = EditorGUILayout.TextField("Piece ID", grid.idPiece);
        if (string.IsNullOrEmpty(grid.idPiece))
            grid.idPiece = null;

        grid.idModifireGrid = EditorGUILayout.TextField("Modifier ID", grid.idModifireGrid);
        if (string.IsNullOrEmpty(grid.idModifireGrid))
            grid.idModifireGrid = null;

        if (oldStatus != grid.stat || oldPiece != grid.idPiece || oldModifier != grid.idModifireGrid)
        {
            Repaint();
        }

        EditorGUI.indentLevel--;
    }

    private void ResizeGrid(MatrixField field, int newSize)
    {
        if (field.data == null)
        {
            field.data = new GridData(newSize, newSize);
            field.size = newSize;
            for (int x = 0; x < newSize; x++)
                for (int y = 0; y < newSize; y++)
                    field.data.SetValue(x, y,new Grid());
            return;
        }

        int oldSize = field.size;

        GridData newData = new(newSize, newSize);

        for (int x = 0; x < newSize; x++)
        {
            for (int y = 0; y < newSize; y++)
            {
                if (x < oldSize && y < oldSize)
                    newData.SetValue(x, y, field.data.GetValue(x, y));
                else
                    newData.SetValue(x, y, new Grid());
            }
        }

        field.data = newData;
        field.size = newSize;
    }

    private void ClearGrid(MatrixField field)
    {
        if (field.data == null) return;

        int size = field.size;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (field.data.GetValue(x, y) != null)
                {
                    field.data.GetValue(x, y).stat = Status.None;
                    field.data.GetValue(x, y).idPiece = null;
                    field.data.GetValue(x, y).idModifireGrid = null;
                }
            }
        }
        Repaint();
    }
}
