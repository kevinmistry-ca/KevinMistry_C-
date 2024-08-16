using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class InvestCloudMatrixMultiplication
{
    private readonly HttpClient _client;

    public InvestCloudMatrixMultiplication(HttpClient client)
    {
        _client = client;
        _client.Timeout = TimeSpan.FromMinutes(5); // Set the timeout to 5 minutes
    }
    public async Task InitializeDatasetsAsync(int size)
    {
        string url = $"https://recruitment-test.investcloud.com/api/numbers/init/{size}";
        HttpResponseMessage response = await _client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to initialize datasets.");
        }
    }

    public async Task<int[]> GetRowOrColAsync(string dataset, string type, int idx)
    {
        try
        {
            string url = $"https://recruitment-test.investcloud.com/api/numbers/{dataset}/{type}/{idx}";
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int[]>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Array.Empty<int>();
        }        
    }

    public int[,] MultiplyMatrices(int[,] A, int[,] B, int size)
    {
        if (size == 1)
        {
            return new int[,] { { A[0, 0] * B[0, 0] } };
        }

        int newSize = size / 2;
        int[,] A11 = new int[newSize, newSize];
        int[,] A12 = new int[newSize, newSize];
        int[,] A21 = new int[newSize, newSize];
        int[,] A22 = new int[newSize, newSize];

        int[,] B11 = new int[newSize, newSize];
        int[,] B12 = new int[newSize, newSize];
        int[,] B21 = new int[newSize, newSize];
        int[,] B22 = new int[newSize, newSize];

        Split(A, A11, 0, 0);
        Split(A, A12, 0, newSize);
        Split(A, A21, newSize, 0);
        Split(A, A22, newSize, newSize);

        Split(B, B11, 0, 0);
        Split(B, B12, 0, newSize);
        Split(B, B21, newSize, 0);
        Split(B, B22, newSize, newSize);

        int[,] M1 = MultiplyMatrices(Add(A11, A22), Add(B11, B22), newSize);
        int[,] M2 = MultiplyMatrices(Add(A21, A22), B11, newSize);
        int[,] M3 = MultiplyMatrices(A11, Subtract(B12, B22), newSize);
        int[,] M4 = MultiplyMatrices(A22, Subtract(B21, B11), newSize);
        int[,] M5 = MultiplyMatrices(Add(A11, A12), B22, newSize);
        int[,] M6 = MultiplyMatrices(Subtract(A21, A11), Add(B11, B12), newSize);
        int[,] M7 = MultiplyMatrices(Subtract(A12, A22), Add(B21, B22), newSize);

        int[,] C11 = Add(Subtract(Add(M1, M4), M5), M7);
        int[,] C12 = Add(M3, M5);
        int[,] C21 = Add(M2, M4);
        int[,] C22 = Add(Subtract(Add(M1, M3), M2), M6);

        int[,] C = new int[size, size];
        Combine(C, C11, 0, 0);
        Combine(C, C12, 0, newSize);
        Combine(C, C21, newSize, 0);
        Combine(C, C22, newSize, newSize);

        return C;
    }

    private void Split(int[,] P, int[,] C, int iB, int jB)
    {
        for (int i = 0, i2 = iB; i < C.GetLength(0); i++, i2++)
            for (int j = 0, j2 = jB; j < C.GetLength(1); j++, j2++)
                C[i, j] = P[i2, j2];
    }

    private void Combine(int[,] P, int[,] C, int iB, int jB)
    {
        for (int i = 0, i2 = iB; i < C.GetLength(0); i++, i2++)
            for (int j = 0, j2 = jB; j < C.GetLength(1); j++, j2++)
                P[i2, j2] = C[i, j];
    }

    private int[,] Add(int[,] A, int[,] B)
    {
        int n = A.GetLength(0);
        int[,] C = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                C[i, j] = A[i, j] + B[i, j];
        return C;
    }

    private int[,] Subtract(int[,] A, int[,] B)
    {
        int n = A.GetLength(0);
        int[,] C = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                C[i, j] = A[i, j] - B[i, j];
        return C;
    }

    public string ConvertMatrixToString(int[,] matrix, int size)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                sb.Append(matrix[i, j]);
            }
        }
        return sb.ToString();
    }

    public string ComputeMd5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Encoding.UTF8.GetString(hashBytes);
        }
    }

    public async Task ValidateResultAsync(string md5Hash)
    {
        var content = new StringContent($"\"{md5Hash}\"", Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync("https://recruitment-test.investcloud.com/api/numbers/validate", content);
        response.EnsureSuccessStatusCode();
        string result = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Validation result: {result}");
    }
}
