using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Domain.Repositories.Interfaces;
using JhipsterDotNetMS.Infrastructure.Data.Extensions;

namespace JhipsterDotNetMS.Infrastructure.Data.Repositories
{
    public class ReadOnlyConferenceRepository : ReadOnlyGenericRepository<Conference, long>, IReadOnlyConferenceRepository
    {
        public ReadOnlyConferenceRepository(IUnitOfWork context) : base(context)
        {
        }
    }
}
