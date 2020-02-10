namespace NetCoreAdmin.Metrics
{
    public class UnknownSystemStatisticsProvider : ISystemStatisticsProvider
    {
        public double GetMetric()
        {
            return 0;
        }
    }
}