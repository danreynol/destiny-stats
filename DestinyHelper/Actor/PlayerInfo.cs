using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public static class PlayerInfo
    {
        /// <summary>
        /// Get instances of a player with name.
        /// </summary>
        /// <param name="name">The name of the clan</param>
        /// <param name="memberType">The type of membership</param>
        /// <returns>The info for the player</returns>
        public static List<Player> GetPlayerInstances(string name, MembershipType memberType)
        {
            string playerDataUrl = string.Format(
                "{0}/Destiny2/SearchDestinyPlayer/{1}/{2}/",
                DestinyClient.UrlBase,
                memberType,
                name);

            dynamic item = DestinyClient.SendRequest(playerDataUrl);

            JArray playerInstances = item.Response;

            List<Player> players = new List<Player>();

            foreach (dynamic playerData in playerInstances)
            {
                string playerId = playerData.membershipId;
                string playerName = playerData.displayName;
                string playerMembershipType = playerData.membershipType;

                Player player = new Player()
                {
                    Id = playerId,
                    Name = playerName,
                    BungieMembershipType = (MembershipType)Convert.ToInt32(playerMembershipType)
                };

                players.Add(player);
            }

            return players;
        }

        /// <summary>
        /// Get a player for PSN.
        /// </summary>
        /// <param name="name">The name of the clan</param>
        /// <returns>The info for the player</returns>
        public static Player GetPsnPlayer(string name)
        {
            return GetPlayer(name, MembershipType.TigerPsn);
        }

        /// <summary>
        /// Get a player for Xbox.
        /// </summary>
        /// <param name="name">The name of the clan</param>
        /// <returns>The info for the player</returns>
        public static Player GetXboxPlayer(string name)
        {
            return GetPlayer(name, MembershipType.TigerXbox);
        }

        /// <summary>
        /// Get a player for Blizzard.
        /// </summary>
        /// <param name="name">The name of the clan</param>
        /// <returns>The info for the player</returns>
        public static Player GetBlizzardPlayer(string name)
        {
            return GetPlayer(name, MembershipType.TigerBlizzard);
        }

        /// <summary>
        /// Get activity history for a character.
        /// </summary>
        /// <param name="memberType">The player membership type</param>
        /// <param name="playerId">The id of the player</param>
        /// <param name="characterId">The id of the character</param>
        /// <param name="countOfActivities">The count of activities to get</param>
        /// <param name="typeOfActivity">The type of activity to get</param>
        /// <returns>List of activities</returns>
        public static List<Activity> GetActivityHistory(
            MembershipType memberType, 
            string playerId, 
            string characterId, 
            int countOfActivities, 
            ActivityModeType typeOfActivity)
        {
            string activityHistoryUrl = string.Format(
                "{0}/Destiny2/{1}/Account/{2}/Character/{3}/Stats/Activities/?count={4}&mode={5}",
                DestinyClient.UrlBase,
                memberType,
                playerId,
                characterId,
                countOfActivities,
                typeOfActivity);

            dynamic item = DestinyClient.SendRequest(activityHistoryUrl);
            List<Activity> activities = new List<Activity>();

            if (item == null)
            {
                return activities;
            }

            JArray activityInstances = item.Response.activities;

            foreach (dynamic activityData in activityInstances)
            {
                DateTime period = Convert.ToDateTime(activityData.period);
                string instanceId = activityData.activityDetails.instanceId;

                List<ActivityModeType> activityTypes = new List<ActivityModeType>();

                JArray activityTypeInstances = activityData.activityDetails.modes;

                foreach (dynamic modeInstance in activityTypeInstances)
                {
                    ActivityModeType activityType = (ActivityModeType)Convert.ToInt32(modeInstance);
                    activityTypes.Add(activityType);
                }

                Activity activity = new Activity()
                {
                    Period = period,
                    InstanceId = instanceId,
                    Modes = activityTypes
                };

                activities.Add(activity);
            }

            return activities;
        }

        /// <summary>
        /// Get a list of players for an activity.
        /// </summary>
        /// <param name="activityId">The id of the activity</param>
        /// <returns>The info for the player</returns>
        public static List<Player> GetPlayersInActivity(string activityId)
        {
            string playerDataUrl = string.Format(
                "{0}/Destiny2/Stats/PostGameCarnageReport/{1}/",
                DestinyClient.UrlBase,
                activityId);

            dynamic item = DestinyClient.SendRequest(playerDataUrl);

            JArray playerInstances = item.Response.entries;

            List<Player> players = new List<Player>();

            foreach (dynamic playerInstance in playerInstances)
            {
                string playerId = playerInstance.player.destinyUserInfo.membershipId;
                string playerName = playerInstance.player.destinyUserInfo.displayName;
                string playerMembershipType = playerInstance.player.destinyUserInfo.membershipType;

                Player player = new Player()
                {
                    Id = playerId,
                    Name = playerName,
                    BungieMembershipType = (MembershipType)Convert.ToInt32(playerMembershipType)
                };

                players.Add(player);
            }

            return players;
        }

        /// <summary>
        /// Get a player instance for a membership type.
        /// </summary>
        /// <param name="name">The name of the clan</param>
        /// <param name="memberType">The type of membership</param>
        /// <returns>The info for the player</returns>
        private static Player GetPlayer(string name, MembershipType memberType)
        {
            string playerDataUrl = string.Format(
                "{0}/Destiny2/SearchDestinyPlayer/{1}/{2}/",
                DestinyClient.UrlBase,
                memberType,
                name);

            dynamic item = DestinyClient.SendRequest(playerDataUrl);

            JArray playerInstances = item.Response;

            Player player = new Player();

            foreach (dynamic playerData in playerInstances)
            {
                string playerId = playerData.membershipId;
                string playerName = playerData.displayName;
                string playerMembershipType = playerData.membershipType;

                player.Id = playerId;
                player.Name = playerName;
                player.BungieMembershipType = (MembershipType)Convert.ToInt32(playerMembershipType);
            }

            return player;
        }
    }
}
