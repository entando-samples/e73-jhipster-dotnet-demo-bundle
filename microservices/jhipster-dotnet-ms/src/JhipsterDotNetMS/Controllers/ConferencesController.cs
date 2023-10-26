using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Crosscutting.Exceptions;
using JhipsterDotNetMS.Dto;
using JhipsterDotNetMS.Web.Extensions;
using JhipsterDotNetMS.Web.Filters;
using JhipsterDotNetMS.Web.Rest.Utilities;
using AutoMapper;
using System.Linq;
using JhipsterDotNetMS.Domain.Services.Interfaces;
using JhipsterDotNetMS.Infrastructure.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace JhipsterDotNetMS.Controllers
{
    [Route("api/conferences")]
    [ApiController]
    // ENTANDO -> Add Authorize Decorator
    [Authorize]
    public class ConferencesController : ControllerBase
    {
        private const string EntityName = "conference";
        private readonly ILogger<ConferencesController> _log;
        private readonly IMapper _mapper;
        private readonly IConferenceService _conferenceService;

        public ConferencesController(ILogger<ConferencesController> log,
        IMapper mapper,
        IConferenceService conferenceService)
        {
            _log = log;
            _mapper = mapper;
            _conferenceService = conferenceService;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<ConferenceDto>> CreateConference([FromBody] ConferenceDto conferenceDto)
        {
            _log.LogDebug($"REST request to save Conference : {conferenceDto}");
            if (conferenceDto.Id != 0)
                throw new BadRequestAlertException("A new conference cannot already have an ID", EntityName, "idexists");

            Conference conference = _mapper.Map<Conference>(conferenceDto);
            await _conferenceService.Save(conference);
            return CreatedAtAction(nameof(GetConference), new { id = conference.Id }, conference)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, conference.Id.ToString()));
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateConference(long id, [FromBody] ConferenceDto conferenceDto)
        {
            _log.LogDebug($"REST request to update Conference : {conferenceDto}");
            if (conferenceDto.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            if (id != conferenceDto.Id) throw new BadRequestAlertException("Invalid Id", EntityName, "idinvalid");
            Conference conference = _mapper.Map<Conference>(conferenceDto);
            await _conferenceService.Save(conference);
            return Ok(conference)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, conference.Id.ToString()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConferenceDto>>> GetAllConferences(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Conferences");
            var result = await _conferenceService.FindAll(pageable);
            var page = new Page<ConferenceDto>(result.Content.Select(entity => _mapper.Map<ConferenceDto>(entity)).ToList(), pageable, result.TotalElements);
            return Ok(((IPage<ConferenceDto>)page).Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConference([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get Conference : {id}");
            var result = await _conferenceService.FindOne(id);
            ConferenceDto conferenceDto = _mapper.Map<ConferenceDto>(result);
            return ActionResultUtil.WrapOrNotFound(conferenceDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConference([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Conference : {id}");
            await _conferenceService.Delete(id);
            return NoContent().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
