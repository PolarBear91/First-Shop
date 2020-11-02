namespace Shop.Domain.Infrastructure
{
    public static class DecimalExtensions
    {
        public static string GetValueString(this int value) =>
            $"${value.ToString("N2")}";
        
    }
}
