using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.DAL.GenericRepository;

namespace BookingQueue.BLL.Services;

public class ServicesService : IServicesService
{
    private readonly IGenericRepository<Common.Models.Services> _genericRepository;

    public ServicesService(IGenericRepository<Common.Models.Services> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<ICollection<Common.Models.Services>> GetAllActiveAsync()
    {
        var result = (await _genericRepository.GetAllAsync()).Where(s => s.Deleted is null && s.Id != 1);
        return result.ToList();
    }
}