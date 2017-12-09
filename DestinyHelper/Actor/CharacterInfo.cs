using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public static class CharacterInfo
    {
        /// <summary>
        /// Get a list of character ids for a player.
        /// </summary>
        /// <param name="playerId">The Id of the player</param>
        /// <param name="memberType">The type of membership</param>
        /// <returns>The info for the player</returns>
        public static List<string> GetCharacterIdsForPlayer(string playerId, MembershipType memberType)
        {
            string characterIdsURL = string.Format(
                "{0}/Destiny2/{1}/Profile/{2}/?components={3}",
                DestinyClient.UrlBase,
                memberType,
                playerId,
                ComponentType.Characters);

            dynamic item = DestinyClient.SendRequest(characterIdsURL);

            JObject dataItem = null;

            try
            {
                dataItem = item.Response.characters.data;
            }
            catch
            {
                return null;
            }

            JEnumerable<JProperty> characterNodes = dataItem.Children<JProperty>();

            List<string> characterIds = new List<string>();

            foreach (JProperty characterNode in characterNodes)
            {
                JEnumerable<JProperty> characterProperties = characterNode.Value.Children<JProperty>();

                foreach (JProperty characterProperty in characterProperties)
                {
                    if (characterProperty.Name == "characterId")
                    {
                        characterIds.Add(characterProperty.Value.ToString());
                        break;
                    }
                }
            }

            return characterIds;
        }
    }
}
