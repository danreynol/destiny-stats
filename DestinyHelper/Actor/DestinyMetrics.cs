using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

using DestinyHelper.Entity;

namespace DestinyHelper.Actor
{
    public class DestinyMetrics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinyMetrics"/> class.
        /// </summary>
        public DestinyMetrics()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DestinyMetrics"/> class.
        /// </summary>
        /// <param name="messageLogger">The message logger to use</param>
        /// <param name="activitiesToGet">The count of activities to get</param>
        public DestinyMetrics(Action<object> messageLogger, int activitiesToGet)
        {
            this.MessageLogger = messageLogger;
            this.ActivitiesToGet = activitiesToGet;
        }

        /// <summary>
        /// Gets or sets the MessageLogger property.
        /// </summary>
        public Action<object> MessageLogger { get; set; }

        /// <summary>
        /// Gets or sets the ActivitiesToGet property.
        /// </summary>
        public int ActivitiesToGet { get; set; }

        /// <summary>
        /// Get clan activity metrics.
        /// </summary>
        /// <param name="clanName">The name of the clan to use</param>
        public void GetClanActivityMetrics(string clanName)
        {
            if (string.IsNullOrEmpty(clanName))
            {
                throw new Exception("Clan name must not be blank");
            }

            try
            {
                string clanId = ClanInfo.GetClanId(clanName);
                List<Player> clanMembers = ClanInfo.GetMemberList(clanId);
                
                this.GetMetricsForActivityType(clanMembers, ActivityModeType.Raid);
                this.GetMetricsForActivityType(clanMembers, ActivityModeType.AllPvP);
                this.GetMetricsForActivityType(clanMembers, ActivityModeType.AllPvE);
                this.GetMetricsForActivityType(clanMembers, ActivityModeType.TrialsOfTheNine);
                this.GetMetricsForActivityType(clanMembers, ActivityModeType.IronBanner);

                this.WriteMessage("Done.");
            }
            catch (Exception ex)
            {
                this.WriteMessage(ex.Message);
            }
        }

        /// <summary>
        /// Create a table to use for results of the clan activity metrics function.
        /// </summary>
        /// <returns>A DataTable to use</returns>
        private DataTable CreateClanActivityMetricsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Player");
            table.Columns.Add("Activity");
            table.Columns.Add("Number of Activities");
            table.Columns.Add("Number of Activities with Clan Members");
            table.Columns.Add("Number of Activities Solo");
            table.Columns.Add("Number of Activities without Clan Members");
            table.Columns.Add("Activities with Clan Members (%)");
            table.Columns.Add("Earliest Period");
            table.Columns.Add("Latest Period");
            
            return table;
        }

        /// <summary>
        /// Get metrics for the activity type specified.
        /// </summary>
        /// <param name="clanMembers">The clan members</param>
        /// <param name="activityType">Activity type to use</param>
        private void GetMetricsForActivityType(List<Player> clanMembers, ActivityModeType activityType)
        {
            DataTable resultsTable = this.CreateClanActivityMetricsTable();
            string activityTypeName = Enum.GetName(typeof(ActivityModeType), activityType);
            int runningCountOfClanMembers = 0;

            foreach (Player clanMember in clanMembers)
            {
                runningCountOfClanMembers++;
                this.WriteMessage(string.Format("Getting {0} data for {1} ({2} out of {3})...", 
                    activityTypeName, 
                    clanMember.Name, 
                    runningCountOfClanMembers, 
                    clanMembers.Count));

                try
                {
                    this.GetPlayerActivityData(resultsTable, activityType, clanMember, clanMembers, activityTypeName);
                }
                catch (Exception ex)
                {
                    this.WriteMessage(ex.Message);
                }
            }

            this.ExportResults(resultsTable, activityTypeName);
        }

        /// <summary>
        /// Export the results to a excel spreadsheet.
        /// </summary>
        /// <param name="resultsTable">The results to export to spreadsheet</param>
        /// <param name="worksheetName">The name of the worksheet to write to</param>
        private void ExportResults(DataTable resultsTable, string worksheetName)
        {
            ExcelExport excelExport = new ExcelExport();
            excelExport.ExportFileName = "Destiny Clan Metrics";
            excelExport.Export(resultsTable, worksheetName);
        }

        /// <summary>
        /// Get player data for the activity.
        /// </summary>
        /// <param name="resultsTable">Table to use</param>
        /// <param name="activityType">The activity type</param>
        /// <param name="playerToCheck">The player</param>
        /// <param name="clanMembers">Clan members</param>
        /// <param name="activityTypeName">The activity type name</param>
        private void GetPlayerActivityData(
            DataTable resultsTable, 
            ActivityModeType activityType, 
            Player playerToCheck, 
            List<Player> clanMembers, 
            string activityTypeName)
        {
            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(playerToCheck.Id, playerToCheck.BungieMembershipType);
            List<Activity> playerActivities = this.GetActivitesForCharacters(characterIds, playerToCheck, activityType);
            ActivityCount activityCount = this.GetActivityCounts(playerActivities, playerToCheck, clanMembers);

            string earliestPeriod = string.Empty;
            string latestPeriod = string.Empty;

            if (playerActivities.Count > 0)
            {
                earliestPeriod = playerActivities[playerActivities.Count - 1].Period.ToString();
                latestPeriod = playerActivities[0].Period.ToString();
            }

            resultsTable.Rows.Add(new object[]
            {
                playerToCheck.Name,
                activityTypeName,
                activityCount.CountOfActivities,
                activityCount.CountOfActivitiesWithClanMembers,
                activityCount.CountOfSoloActivities,
                activityCount.CountOfActivitieswithoutClanMembers,
                activityCount.PercentPlayedWithClanMembers,
                earliestPeriod,
                latestPeriod
            });
        }

        /// <summary>
        /// Gets a list of activities for the characters.
        /// </summary>
        /// <param name="characterIds">List of characters</param>
        /// <param name="playerToCheck">The player to check</param>
        /// <param name="activityType">The activity mode type</param>
        /// <returns>A list of activities</returns>
        private List<Activity> GetActivitesForCharacters(List<string> characterIds, Player playerToCheck, ActivityModeType activityType)
        {
            List<Activity> playerActivities = new List<Activity>();

            foreach (string characterId in characterIds)
            {
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    playerToCheck.BungieMembershipType,
                    playerToCheck.Id,
                    characterId,
                    this.ActivitiesToGet,
                    activityType);

                // Sleep the thread so that Bungie doesn't reject frequent requests.
                Thread.Sleep(100);

                playerActivities.AddRange(activities);
            }

            return playerActivities;
        }

        /// <summary>
        /// Gets the activity counts for a player.
        /// </summary>
        /// <param name="playerActivities">The list of activities</param>
        /// <param name="playerToCheck">The player to check</param>
        /// <param name="clanMembers">The list of clan members</param>
        /// <returns>The activity count</returns>
        private ActivityCount GetActivityCounts(List<Activity> playerActivities, Player playerToCheck, List<Player> clanMembers)
        {
            int countOfActivities = 0;
            int countOfSoloActivities = 0;
            int countOfActivitiesWithClanMembers = 0;

            foreach (Activity activity in playerActivities)
            {
                countOfActivities++;

                List<Player> playersInActivity = PlayerInfo.GetPlayersInActivity(activity.InstanceId);

                if (playersInActivity.Count == 1)
                {
                    countOfSoloActivities++;
                    continue;
                }

                foreach (Player playerInActivity in playersInActivity)
                {
                    if (playerInActivity.Name == playerToCheck.Name)
                    {
                        continue;
                    }

                    bool foundMatch = false;

                    foreach (Player clanMember in clanMembers)
                    {
                        if (playerInActivity.Id == clanMember.Id)
                        {
                            countOfActivitiesWithClanMembers++;
                            foundMatch = true;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        break;
                    }
                }
            }

            ActivityCount activityCount = new ActivityCount()
            {
                CountOfActivities = countOfActivities,
                CountOfSoloActivities = countOfSoloActivities,
                CountOfActivitiesWithClanMembers = countOfActivitiesWithClanMembers
            };

            return activityCount;
        }

        /// <summary>
        /// Send a message to message logger.
        /// </summary>
        /// <param name="message">Message to log.</param>
        private void WriteMessage(object message)
        {
            if (message != null)
            {
                this.MessageLogger?.Invoke(message);
            }
        }
    }
}
