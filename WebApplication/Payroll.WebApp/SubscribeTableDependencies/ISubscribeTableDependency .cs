namespace Payroll.WebApp.SubscribeTableDependencies
{
    public interface ISubscribeTableDependency
    {
        Task SubscribeTableDependencyNotificationAsync(string connectionString);
    }
}
