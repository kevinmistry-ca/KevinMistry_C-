
public class Program
{
    public static async Task Main(string[] args)
    {
        const int size = 1000;
        using HttpClient client = new HttpClient();
        InvestCloudMatrixMultiplication investCloudMatrix = new InvestCloudMatrixMultiplication(client);

        //Initialize call here
        await investCloudMatrix.InitializeDatasetsAsync(size);

        int newSize = size / 2;

        // Fetch submatrices for A
        int[,] A11 = await FetchSubMatrix(investCloudMatrix, "A", 0, 0, newSize);
        int[,] A12 = await FetchSubMatrix(investCloudMatrix, "A", 0, newSize, newSize);
        int[,] A21 = await FetchSubMatrix(investCloudMatrix, "A", newSize, 0, newSize);
        int[,] A22 = await FetchSubMatrix(investCloudMatrix, "A", newSize, newSize, newSize);

        // Fetch submatrices for B
        int[,] B11 = await FetchSubMatrix(investCloudMatrix, "B", 0, 0, newSize);
        int[,] B12 = await FetchSubMatrix(investCloudMatrix, "B", 0, newSize, newSize);
        int[,] B21 = await FetchSubMatrix(investCloudMatrix, "B", newSize, 0, newSize);
        int[,] B22 = await FetchSubMatrix(investCloudMatrix, "B", newSize, newSize, newSize);

        // Step 3: Perform Strassen matrix multiplication
        int[,] C11 = investCloudMatrix.MultiplyMatrices(A11, B11, newSize);
        int[,] C12 = investCloudMatrix.MultiplyMatrices(A12, B21, newSize);
        int[,] C21 = investCloudMatrix.MultiplyMatrices(A21, B12, newSize);
        int[,] C22 = investCloudMatrix.MultiplyMatrices(A22, B22, newSize);

        // Combine result
        int[,] C = new int[size, size];
        CombineMatrices(C, C11, 0, 0);
        CombineMatrices(C, C12, 0, newSize);
        CombineMatrices(C, C21, newSize, 0);
        CombineMatrices(C, C22, newSize, newSize);

        //Output Validation
        string concatenatedMatrixString = investCloudMatrix.ConvertMatrixToString(C, size);
        string md5Hash = investCloudMatrix.ComputeMd5Hash(concatenatedMatrixString);

        await investCloudMatrix.ValidateResultAsync(md5Hash);
    }

    private static async Task<int[,]> FetchSubMatrix(InvestCloudMatrixMultiplication investCloudMatrix, string dataset, int startRow, int startCol, int size)
    {
        int[,] subMatrix = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            // Fetch the entire row for the current submatrix row
            int[] row = await investCloudMatrix.GetRowOrColAsync(dataset, "row", startRow + i);

            // Check that startCol + j does not exceed the row length
            for (int j = 0; j < size; j++)
            {
                subMatrix[i, j] = row[startCol + j]; // Access the correct column within the submatrix
            }
        }
        return subMatrix;
    }

    private static void CombineMatrices(int[,] P, int[,] C, int iB, int jB)
    {
        for (int i = 0, i2 = iB; i < C.GetLength(0); i++, i2++)
            for (int j = 0, j2 = jB; j < C.GetLength(1); j++, j2++)
                P[i2, j2] = C[i, j];
    }
}