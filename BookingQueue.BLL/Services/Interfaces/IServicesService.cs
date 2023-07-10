namespace BookingQueue.BLL.Services.Interfaces;

public interface IServicesService
{
    Task<ICollection<Common.Models.Services>> GetAllActiveAsync();
}