using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using JhipsterDotNetMS.Domain.Entities;

namespace JhipsterDotNetMS.Domain.Services.Interfaces
{
    public interface IConferenceService
    {
        Task<Conference> Save(Conference conference);

        Task<IPage<Conference>> FindAll(IPageable pageable);

        Task<Conference> FindOne(long id);

        Task Delete(long id);
    }
}
