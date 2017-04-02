﻿using System.Collections.Generic;
using System.Linq;
using TeamScreen.Models.TeamCity;
using TeamScreen.TeamCity;

namespace TeamScreen.Services.TeamCity
{
    public interface IBuildMapper
    {
        Dictionary<string, TeamCityBuildModel[]> Map(IEnumerable<BuildJob> buildJobs);
    }

    public class BuildMapper : IBuildMapper
    {
        public Dictionary<string, TeamCityBuildModel[]> Map(IEnumerable<BuildJob> buildJobs)
        {
            return buildJobs
                .GroupBy(x => x.Project.Name)
                .ToDictionary(x => x.Key, y => y.Select(MapSingleBuild).ToArray());
        }

        public TeamCityBuildModel MapSingleBuild(BuildJob buildJob)
        {
            var lastBuild = buildJob.BuildCollection.Builds.First();
            return new TeamCityBuildModel
            {
                Name = buildJob.Name,
                Date = lastBuild.FinishDate ?? lastBuild.StartDate,
                Status = MapStatus(lastBuild),
                TriggeredBy = MapTriggeredBy(lastBuild.Trigger)
            };
        }

        public TeamCityStatusModel MapStatus(Build build)
        {
            if (build.State != BuildState.Finished)
                return TeamCityStatusModel.Pending;
            else if (build.Status != BuildStatus.Success)
                return TeamCityStatusModel.Failure;
            else
                return TeamCityStatusModel.Success;
        }

        public string MapTriggeredBy(BuildTrigger trigger)
        {
            if (trigger.Type == TriggerType.User)
                return trigger.User.Name;
            else if (trigger.Type == TriggerType.BuildType)
                return trigger.DependantBuild?.Name;
            else
                return "Source control";
        }
    }
}