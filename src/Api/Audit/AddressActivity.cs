namespace ClassManager.Api.Audit;

internal sealed class AddressActivity
{
    private int _recorded;
    private int _unauthorized;
    private int _suppressed;

    public int Unauthorized => _unauthorized;

    public int Suppressed => _suppressed;

    public int RecordOne() => Interlocked.Increment(ref _recorded);

    public void CountUnauthorized() => Interlocked.Increment(ref _unauthorized);

    public void CountSuppressed() => Interlocked.Increment(ref _suppressed);
}
