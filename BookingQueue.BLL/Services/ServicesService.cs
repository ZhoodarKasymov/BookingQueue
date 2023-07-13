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
        var query = @"SELECT s.id, s.name, sl.name as 'TranslatedName', s.deleted, sl.lang FROM services_langs sl
                        RIGHT JOIN services s ON s.id = sl.services_id
                        WHERE s.deleted IS NULL && sl.lang = 'kz_KZ'";

        var services = await _genericRepository.QueryDynamicAsync<Common.Models.Services>(query);
        return services.ToList();
    }
}