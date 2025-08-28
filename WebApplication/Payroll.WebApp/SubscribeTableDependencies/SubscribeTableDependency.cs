using Payroll.WebApp.Models;
using Payroll.WebApp.SignalRHubs;
using TableDependency.SqlClient;

namespace Payroll.WebApp.SubscribeTableDependencies
{
    //public class SubscribeTableDependency : ISubscribeTableDependency
    //{
    //    private SqlTableDependency<Notificationcs> tableDependency;
    //    private readonly NotificationHub notificationHub;
    //    private readonly NotificationService _notificationService;

    //    public SubscribeTableDependency(NotificationHub notificationHub, NotificationService notificationService)
    //    {
    //        this.notificationHub = notificationHub;
    //        _notificationService = notificationService;
    //    }

    //    public Task SubscribeTableDependencyNotificationAsync(string connectionString)
    //    {
    //        tableDependency = new SqlTableDependency<Notificationcs>(connectionString, tableName: "tbl_gen_notification");

    //        // Subscribe to the events without using 'await'
    //        tableDependency.OnChanged += TableDependency_OnChanged;
    //        tableDependency.OnError += TableDependency_OnError;

    //        // Start the dependency
    //        tableDependency.Start();

    //        return Task.CompletedTask; // Return a completed task
    //    }



    //    private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Notificationcs> e)
    //    {
    //        if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
    //        {
    //            // Handle the notifications asynchronously
    //            await _notificationService.GetActiveNotificationsAsync();
    //            await notificationHub.SendNotiFicationCount();
    //        }
    //    }


    //    private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
    //    {
    //        Console.WriteLine($"{nameof(Notificationcs)} SqlTableDependency error: {e.Error.Message}");
    //    }
    //}
}
