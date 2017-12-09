using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public static class ClanInfo
    {
        /// <summary>
        /// Get the Id of a given clan.
        /// </summary>
        /// <param name="clanName">The name of the clan</param>
        /// <returns>The Id of the clan</returns>
        public static string GetClanId(string clanName)
        {
            string clanURL = string.Format(
                "{0}/GroupV2/Name/{1}/{2}/",
                DestinyClient.UrlBase,
                clanName,
                GroupType.Clan);

            dynamic item = DestinyClient.SendRequest(clanURL);
            string clanId = item.Response.detail.groupId;

            return clanId;
        }

        /// <summary>
        /// Get the members of a clan.
        /// </summary>
        /// <param name="clanId">The Id of the clan</param>
        /// <returns>List of members</returns>
        public static List<Player> GetMemberList(string clanId)
        {
            string memberListURL = string.Format(
                "{0}/GroupV2/{1}/Members/?currentPage=1",
                DestinyClient.UrlBase,
                clanId);

            dynamic item = DestinyClient.SendRequest(memberListURL);
            JArray listOfMembers = item.Response.results;
            List<Player> clanMembers = new List<Player>();

            foreach (dynamic memberItem in listOfMembers)
            {
                string id = memberItem.destinyUserInfo.membershipId;
                string name = memberItem.destinyUserInfo.displayName;
                string membershipType = memberItem.destinyUserInfo.membershipType;

                Player member = new Player()
                {
                    Id = id,
                    Name = name,
                    BungieMembershipType = (MembershipType)Convert.ToInt32(membershipType)
                };

                clanMembers.Add(member);
            }

            return clanMembers;
        }
    }
}
