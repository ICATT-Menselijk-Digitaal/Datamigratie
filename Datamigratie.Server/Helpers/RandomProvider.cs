namespace Datamigratie.Server.Helpers
{
    public interface IRandomProvider
    {
        int Next(int min, int max);
        double NextDouble();
    }

    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random = new();
        public int Next(int min, int max) => _random.Next(min, max);
        public double NextDouble() => _random.NextDouble();
    }
}
