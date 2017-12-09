using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public static class StatsInfo
    {
        /// <summary>
        /// Get the historical stats for an account.
        /// </summary>
        /// <param name="memberType">The membership type</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The Id of the clan</returns>
        public static string GetHistoricalStatsForAccount(MembershipType memberType, string playerId)
        {
            string statsUrl = string.Format(
                "{0}/Destiny2/{1}/Account/{2}/Stats/?groups={3}",
                DestinyClient.UrlBase,
                memberType,
                playerId,
                StatsGroupType.Activity);

            dynamic item = DestinyClient.SendRequest(statsUrl);

            return item.ToString();
        }

        /// <summary>
        /// Get the raid completions for an account.
        /// </summary>
        /// <param name="memberType">The membership type</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The number of raid completions</returns>
        public static int GetRaidCompletionsForAccount(MembershipType memberType, string playerId)
        {
            int raidCompletions = 0;
            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(playerId, memberType);

            if (characterIds == null || characterIds.Count == 0)
            {
                return 0;
            }

            foreach (string characterId in characterIds)
            {
                string statsUrl = string.Format(
                    "{0}/Destiny2/{1}/Account/{2}/Character/{3}/Stats/?modes={4}",
                    DestinyClient.UrlBase,
                    memberType,
                    playerId,
                    characterId,
                    ActivityModeType.Raid);

                dynamic item = DestinyClient.SendRequest(statsUrl);
                JObject raidItem = item.Response.raid;

                if (raidItem.HasValues)
                {
                    dynamic activitiesCleared = item.Response.raid.allTime.activitiesCleared.basic.value;
                    raidCompletions += Convert.ToInt32(activitiesCleared);
                }
            }

            return raidCompletions;
        }
    }
}
