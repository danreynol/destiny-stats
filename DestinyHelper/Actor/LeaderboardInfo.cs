using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public static class LeaderboardInfo
    {
        /// <summary>
        /// Get the prestige nightfall leaderboard.
        /// </summary>
        /// <param name="clanId">The id of the clan</param>
        /// <param name="maxTopSetting">The max top number to get</param>
        /// <returns>The list of the leaders</returns>
        public static List<LeaderboardSection> GetPrestigeNightfallLeaderboard(string clanId, int maxTopSetting)
        {
            string leaderboardURL = string.Format(
                "{0}/Destiny2/Stats/Leaderboards/Clans/{1}/?maxtop={2}&modes={3}",
                DestinyClient.UrlBase,
                clanId,
                maxTopSetting,
                ActivityModeType.HeroicNightfall);

            dynamic item = DestinyClient.SendRequest(leaderboardURL);
            List<LeaderboardSection> leaderSections = new List<LeaderboardSection>();

            LeaderboardSection singleGameKillsSection = GetLeaderboardSection("Single Game Kills", item.Response.heroicNightfall.lbSingleGameKills);
            LeaderboardSection precisionKillsSection = GetLeaderboardSection("Precision Kills", item.Response.heroicNightfall.lbPrecisionKills);
            LeaderboardSection assistsSection = GetLeaderboardSection("Assists", item.Response.heroicNightfall.lbAssists);
            LeaderboardSection deathsSection = GetLeaderboardSection("Deaths", item.Response.heroicNightfall.lbDeaths);
            LeaderboardSection killsSection = GetLeaderboardSection("Kills", item.Response.heroicNightfall.lbKills);
            LeaderboardSection objectivesCompletedSection = GetLeaderboardSection("Objectives Completed", item.Response.heroicNightfall.lbObjectivesCompleted);
            LeaderboardSection mostPrecisionKillsSection = GetLeaderboardSection("Most Precision Kills", item.Response.heroicNightfall.lbMostPrecisionKills);
            LeaderboardSection longestKillingSpreeSection = GetLeaderboardSection("Longest Killing Spree", item.Response.heroicNightfall.lbLongestKillSpree);
            LeaderboardSection longestKillDistanceSection = GetLeaderboardSection("Longest Kill Distance", item.Response.heroicNightfall.lbLongestKillDistance);
            LeaderboardSection fastestCompletionSection = GetLeaderboardSection("Fastest Completion", item.Response.heroicNightfall.lbFastestCompletionMs);
            LeaderboardSection longestSingleLifeSection = GetLeaderboardSection("Longest Single Life", item.Response.heroicNightfall.lbLongestSingleLife);

            leaderSections.Add(singleGameKillsSection);
            leaderSections.Add(precisionKillsSection);
            leaderSections.Add(assistsSection);
            leaderSections.Add(deathsSection);
            leaderSections.Add(killsSection);
            leaderSections.Add(objectivesCompletedSection);
            leaderSections.Add(mostPrecisionKillsSection);
            leaderSections.Add(longestKillingSpreeSection);
            leaderSections.Add(longestKillDistanceSection);
            leaderSections.Add(fastestCompletionSection);
            leaderSections.Add(longestSingleLifeSection);

            return leaderSections;
        }

        /// <summary>
        /// Get the raid leaderboard.
        /// </summary>
        /// <param name="clanId">The id of the clan</param>
        /// <param name="maxTopSetting">The max top number to get</param>
        /// <returns>The list of the leaders</returns>
        public static List<LeaderboardSection> GetRaidLeaderboard(string clanId, int maxTopSetting)
        {
            string leaderboardURL = string.Format(
                "{0}/Destiny2/Stats/Leaderboards/Clans/{1}/?maxtop={2}&modes={3}",
                DestinyClient.UrlBase,
                clanId,
                maxTopSetting,
                ActivityModeType.Raid);

            dynamic item = DestinyClient.SendRequest(leaderboardURL);
            List<LeaderboardSection> leaderSections = new List<LeaderboardSection>();

            LeaderboardSection singleGameKillsSection = GetLeaderboardSection("Single Game Kills", item.Response.raid.lbSingleGameKills);
            LeaderboardSection precisionKillsSection = GetLeaderboardSection("Precision Kills", item.Response.raid.lbPrecisionKills);
            LeaderboardSection assistsSection = GetLeaderboardSection("Assists", item.Response.raid.lbAssists);
            LeaderboardSection deathsSection = GetLeaderboardSection("Deaths", item.Response.raid.lbDeaths);
            LeaderboardSection killsSection = GetLeaderboardSection("Kills", item.Response.raid.lbKills);
            LeaderboardSection objectivesCompletedSection = GetLeaderboardSection("Objectives Completed", item.Response.raid.lbObjectivesCompleted);
            LeaderboardSection mostPrecisionKillsSection = GetLeaderboardSection("Most Precision Kills", item.Response.raid.lbMostPrecisionKills);
            LeaderboardSection longestKillingSpreeSection = GetLeaderboardSection("Longest Killing Spree", item.Response.raid.lbLongestKillSpree);
            LeaderboardSection longestKillDistanceSection = GetLeaderboardSection("Longest Kill Distance", item.Response.raid.lbLongestKillDistance);
            LeaderboardSection fastestCompletionSection = GetLeaderboardSection("Fastest Completion", item.Response.raid.lbFastestCompletionMs);
            LeaderboardSection longestSingleLifeSection = GetLeaderboardSection("Longest Single Life", item.Response.raid.lbLongestSingleLife);

            leaderSections.Add(singleGameKillsSection);
            leaderSections.Add(precisionKillsSection);
            leaderSections.Add(assistsSection);
            leaderSections.Add(deathsSection);
            leaderSections.Add(killsSection);
            leaderSections.Add(objectivesCompletedSection);
            leaderSections.Add(mostPrecisionKillsSection);
            leaderSections.Add(longestKillingSpreeSection);
            leaderSections.Add(longestKillDistanceSection);
            leaderSections.Add(fastestCompletionSection);
            leaderSections.Add(longestSingleLifeSection);

            return leaderSections;
        }

        /// <summary>
        /// Get the leaderboard section from the response.
        /// </summary>
        /// <param name="name">The name of the section</param>
        /// <param name="section">The section to evaluate</param>
        /// <returns>A leaderboard section</returns>
        private static LeaderboardSection GetLeaderboardSection(string name, dynamic section)
        {
            LeaderboardSection leaderboardSection = new LeaderboardSection()
            {
                Name = name
            };

            List<LeaderboardEntry> leaders = new List<LeaderboardEntry>();
            JArray entries = section.entries;

            foreach (dynamic entry in entries)
            {
                string player = entry.player.destinyUserInfo.displayName;
                string rank = entry.rank;
                string value = entry.value.basic.displayValue;

                LeaderboardEntry leaderEntry = new LeaderboardEntry()
                {
                    Rank = Convert.ToInt32(rank),
                    Player = player,
                    Value = value
                };

                leaders.Add(leaderEntry);
            }

            leaderboardSection.LeaderboardEntries = leaders;

            return leaderboardSection;
        }
    }
}
