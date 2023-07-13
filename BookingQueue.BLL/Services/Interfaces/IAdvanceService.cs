using BookingQueue.Common.Models.ViewModels;

namespace BookingQueue.BLL.Services
{
    public interface IAdvanceService
    {
        Task<string> BookTimeAsync(BookViewModel bookViewModel, int maxClients);
    }
}