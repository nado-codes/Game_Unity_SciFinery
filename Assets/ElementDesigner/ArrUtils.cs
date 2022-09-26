using System;

public static class ArrUtils
{
    public static T[] First<T>(this T[,] arr, Func<T[], bool> func = null)
    {
        if (func == null) return arr.GetRow(0);

        var cursor = 0;
        while (cursor < arr.NumRows())
        {
            var row = arr.GetRow(cursor);

            if (func(row))
                return row;

            cursor++;
        }

        return null;
    }

    public static int NumRows<T>(this T[,] arr)
    {
        var cursor = 1;
        var rows = 0;

        while (cursor < arr.Length)
        {
            cursor += arr.GetLength(rows);
            rows++;
        }

        return rows;
    }

    public static T[] GetRow<T>(this T[,] arr, int index)
    {
        var size = arr.GetLength(0) + 1;
        var output = new T[size];
        for (int i = 0; i < size; ++i)
            output[i] = arr[index, i];

        return output;
    }
}