using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Domain.Services.Interfaces;
using JhipsterDotNetMS.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JhipsterDotNetMS.Domain.Services;

public class ConferenceService : IConferenceService
{
    protected readonly IConferenceRepository _conferenceRepository;

    public ConferenceService(IConferenceRepository conferenceRepository)
    {
        _conferenceRepository = conferenceRepository;
    }

    public virtual async Task<Conference> Save(Conference conference)
    {
        await _conferenceRepository.CreateOrUpdateAsync(conference);
        await _conferenceRepository.SaveChangesAsync();
        return conference;
    }

    public virtual async Task<IPage<Conference>> FindAll(IPageable pageable)
    {
        var page = await _conferenceRepository.QueryHelper()
            .GetPageAsync(pageable);
        return page;
    }

    public virtual async Task<Conference> FindOne(long id)
    {
        var result = await _conferenceRepository.QueryHelper()
            .GetOneAsync(conference => conference.Id == id);
        return result;
    }

    public virtual async Task Delete(long id)
    {
        await _conferenceRepository.DeleteByIdAsync(id);
        await _conferenceRepository.SaveChangesAsync();
    }
}
