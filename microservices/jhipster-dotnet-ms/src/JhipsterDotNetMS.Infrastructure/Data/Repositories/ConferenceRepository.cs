using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Domain.Repositories.Interfaces;
using JhipsterDotNetMS.Infrastructure.Data.Extensions;

namespace JhipsterDotNetMS.Infrastructure.Data.Repositories
{
    public class ConferenceRepository : GenericRepository<Conference, long>, IConferenceRepository
    {
        public ConferenceRepository(IUnitOfWork context) : base(context)
        {
        }

    }
}
