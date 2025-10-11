namespace ApiLabo;

public static class DisplayIdGenerator
{
    private static readonly int _currentId = 0;

    public static string GenerateDisplayId(string prefix, int size = 12)
    {
        var r = new Random();
        var id = r.Next(0, 999999999).ToString("D" + size);
        return $"{prefix}_{id}";
    }
}
