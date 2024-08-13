using MultiThreading.Task3.MatrixMultiplier.Matrices;

namespace MultiThreading.Task3.MatrixMultiplier
{
    public static class MatrixHelper
    {
        public static void InitMatrix(this IMatrix matrix, long[,] matrixInitData)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            { 
                for (int j = 0; j < matrix.ColCount; j++)
                {
                    matrix.SetElement(i, j, matrixInitData[i, j]);
                }
            }
        }
    }
}
