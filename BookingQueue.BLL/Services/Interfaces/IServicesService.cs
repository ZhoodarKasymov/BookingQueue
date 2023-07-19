namespace BookingQueue.BLL.Services.Interfaces;

public interface IServicesService
{
    Task<ICollection<Common.Models.Services>> GetAllActiveAsync();

    Task<List<string>> GetTimeWithPeriodByDate(DateTime? date, long? serviceId);
}