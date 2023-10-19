
using AutoMapper;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JhipsterDotNetMS.Infrastructure.Data;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Domain.Repositories.Interfaces;
using JhipsterDotNetMS.Dto;
using JhipsterDotNetMS.Configuration.AutoMapper;
using JhipsterDotNetMS.Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JhipsterDotNetMS.Test.Controllers
{
    public class ConferencesControllerIntTest
    {
        public ConferencesControllerIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _conferenceRepository = _factory.GetRequiredService<IConferenceRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = config.CreateMapper();

            InitTest();
        }

        private const string DefaultName = "AAAAAAAAAA";
        private const string UpdatedName = "BBBBBBBBBB";

        private const string DefaultLocation = "AAAAAAAAAA";
        private const string UpdatedLocation = "BBBBBBBBBB";

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly IConferenceRepository _conferenceRepository;

        private Conference _conference;

        private readonly IMapper _mapper;

        private Conference CreateEntity()
        {
            return new Conference
            {
                Name = DefaultName,
                Location = DefaultLocation,
            };
        }

        private void InitTest()
        {
            _conference = CreateEntity();
        }

        [Fact]
        public async Task CreateConference()
        {
            var databaseSizeBeforeCreate = await _conferenceRepository.CountAsync();

            // Create the Conference
            ConferenceDto _conferenceDto = _mapper.Map<ConferenceDto>(_conference);
            var response = await _client.PostAsync("/api/conferences", TestUtil.ToJsonContent(_conferenceDto));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Conference in the database
            var conferenceList = await _conferenceRepository.GetAllAsync();
            conferenceList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testConference = conferenceList.Last();
            testConference.Name.Should().Be(DefaultName);
            testConference.Location.Should().Be(DefaultLocation);
        }

        [Fact]
        public async Task CreateConferenceWithExistingId()
        {
            var databaseSizeBeforeCreate = await _conferenceRepository.CountAsync();
            // Create the Conference with an existing ID
            _conference.Id = 1L;

            // An entity with an existing ID cannot be created, so this API call must fail
            ConferenceDto _conferenceDto = _mapper.Map<ConferenceDto>(_conference);
            var response = await _client.PostAsync("/api/conferences", TestUtil.ToJsonContent(_conferenceDto));

            // Validate the Conference in the database
            var conferenceList = await _conferenceRepository.GetAllAsync();
            conferenceList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task GetAllConferences()
        {
            // Initialize the database
            await _conferenceRepository.CreateOrUpdateAsync(_conference);
            await _conferenceRepository.SaveChangesAsync();

            // Get all the conferenceList
            var response = await _client.GetAsync("/api/conferences?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_conference.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
            json.SelectTokens("$.[*].location").Should().Contain(DefaultLocation);
        }

        [Fact]
        public async Task GetConference()
        {
            // Initialize the database
            await _conferenceRepository.CreateOrUpdateAsync(_conference);
            await _conferenceRepository.SaveChangesAsync();

            // Get the conference
            var response = await _client.GetAsync($"/api/conferences/{_conference.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_conference.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
            json.SelectTokens("$.location").Should().Contain(DefaultLocation);
        }

        [Fact]
        public async Task GetNonExistingConference()
        {
            var maxValue = long.MaxValue;
            var response = await _client.GetAsync("/api/conferences/" + maxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateConference()
        {
            // Initialize the database
            await _conferenceRepository.CreateOrUpdateAsync(_conference);
            await _conferenceRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _conferenceRepository.CountAsync();

            // Update the conference
            var updatedConference = await _conferenceRepository.QueryHelper().GetOneAsync(it => it.Id == _conference.Id);
            // Disconnect from session so that the updates on updatedConference are not directly saved in db
            //TODO detach
            updatedConference.Name = UpdatedName;
            updatedConference.Location = UpdatedLocation;

            ConferenceDto updatedConferenceDto = _mapper.Map<ConferenceDto>(updatedConference);
            var response = await _client.PutAsync($"/api/conferences/{_conference.Id}", TestUtil.ToJsonContent(updatedConferenceDto));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Conference in the database
            var conferenceList = await _conferenceRepository.GetAllAsync();
            conferenceList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testConference = conferenceList.Last();
            testConference.Name.Should().Be(UpdatedName);
            testConference.Location.Should().Be(UpdatedLocation);
        }

        [Fact]
        public async Task UpdateNonExistingConference()
        {
            var databaseSizeBeforeUpdate = await _conferenceRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            ConferenceDto _conferenceDto = _mapper.Map<ConferenceDto>(_conference);
            var response = await _client.PutAsync("/api/conferences/1", TestUtil.ToJsonContent(_conferenceDto));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Conference in the database
            var conferenceList = await _conferenceRepository.GetAllAsync();
            conferenceList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteConference()
        {
            // Initialize the database
            await _conferenceRepository.CreateOrUpdateAsync(_conference);
            await _conferenceRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _conferenceRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/conferences/{_conference.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Validate the database is empty
            var conferenceList = await _conferenceRepository.GetAllAsync();
            conferenceList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Conference));
            var conference1 = new Conference
            {
                Id = 1L
            };
            var conference2 = new Conference
            {
                Id = conference1.Id
            };
            conference1.Should().Be(conference2);
            conference2.Id = 2L;
            conference1.Should().NotBe(conference2);
            conference1.Id = 0;
            conference1.Should().NotBe(conference2);
        }
    }
}
