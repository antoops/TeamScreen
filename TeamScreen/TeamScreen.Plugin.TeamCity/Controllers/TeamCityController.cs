using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TeamScreen.Plugin.Base;
using TeamScreen.Plugin.TeamCity.Integration;
using TeamScreen.Plugin.TeamCity.Mapping;

namespace TeamScreen.Plugin.TeamCity.Controllers
{
    public class TeamCityController : PluginControllerBase
    {
        private readonly ITeamCityService _teamCityService;
        private readonly IBuildMapper _buildMapper;
        private readonly IConfigurationRoot _configurationRoot;

        public TeamCityController(ITeamCityService teamCityService, IConfigurationRoot configurationRoot, IBuildMapper buildMapper)
        {
            _teamCityService = teamCityService;
            _configurationRoot = configurationRoot;
            _buildMapper = buildMapper;
        }

        public override string Name { get; } = "TeamCity";

        public override async Task<PartialViewResult> Content()
        {
            var url = _configurationRoot["TeamCityUrl"];
            var username = _configurationRoot["TeamCityUsername"];
            var password = _configurationRoot["TeamCityPassword"];
            var builds = await _teamCityService.GetBuilds(url, username, password);

            var models = _buildMapper.Map(builds);
            return PartialView(models);
        }
    }
}