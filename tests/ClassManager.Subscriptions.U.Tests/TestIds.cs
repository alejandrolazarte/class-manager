namespace ClassManager.Subscriptions.U.Tests;

internal static class TestIds
{
    private static long _lastId = 1000;

    public static long Next() => Interlocked.Increment(ref _lastId);
}
