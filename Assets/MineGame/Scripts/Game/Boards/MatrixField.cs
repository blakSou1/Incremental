using System;

[Serializable]
public class MatrixField
{
    public GridData data;

    public int size;

    public MatrixField(int size)
    {
        data = new(size, size);
        this.size = size;
    }
}

[Serializable]
public class GridData
{
    public int width;
    public int height;
    public Grid[] data; // Одномерный массив для сериализации

    public GridData(int w, int h)
    {
        width = w;
        height = h;
        data = new Grid[w * h];
    }

    // Метод для доступа по координатам [x, y]
    public Grid GetValue(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return data[y * width + x];

        throw new ArgumentOutOfRangeException("Координаты вышли за пределы сетки" + x + y);
    }

    // Метод для записи значения
    public void SetValue(int x, int y, Grid value)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            data[y * width + x] = value;
        else
            throw new ArgumentOutOfRangeException("Координаты вышли за пределы сетки");
    }
}

[Serializable]
public class Grid
{
    public Status stat = Status.None;

    public string idPiece = null;
    public string idModifireGrid = null;
}

public enum Status { Black = -1, None = 0, White = 1 }
