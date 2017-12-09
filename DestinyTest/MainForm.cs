using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using DestinyHelper.Entity;
using DestinyHelper.Actor;

using OfficeOpenXml;

namespace DestinyTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void GetClanActivityMetrics()
        {
            string clanName = "My Clan Name";
            int activitiesToGet = 250;
            DestinyMetrics destinyMetrics = new DestinyMetrics(this.WriteMessage, activitiesToGet);
            destinyMetrics.GetClanActivityMetrics(clanName);
        }

        private void GetClanActivityMetrics2()
        {
            try
            {
                this.WriteMessage("Starting function...");

                string clanName = "My Clan Name";
                string clanId = ClanInfo.GetClanId(clanName);

                List<Player> clanMembers = ClanInfo.GetMemberList(clanId);

                
                Player player1 = PlayerInfo.GetPsnPlayer("psnPlayer");
                Player player2 = PlayerInfo.GetPsnPlayer("psnPlayer2");
                Player player3 = PlayerInfo.GetPsnPlayer("psnPlayer3");

                List<Player> playersToCheck = new List<Player>();
                playersToCheck.Add(player1);
                playersToCheck.Add(player2);
                playersToCheck.Add(player3);
                

                DataTable table = new DataTable();
                table.Columns.Add("Player");
                table.Columns.Add("Activity");
                table.Columns.Add("Number of Activities");
                table.Columns.Add("Number of Activities with Clan Members");
                table.Columns.Add("Number of Activities Solo");
                table.Columns.Add("Number of Activities without Clan Members");
                table.Columns.Add("Earliest Period");

                int count = 0;

                foreach (Player clanMember in playersToCheck)
                {
                    count++;
                    this.WriteMessage(string.Format("Getting data for {0} ({1} out of {2})...", clanMember.Name, count, clanMembers.Count));

                    try
                    {
                        this.GetPlayerActivityData(table, ActivityModeType.Raid, clanMember, clanMembers);
                        this.GetPlayerActivityData(table, ActivityModeType.AllPvP, clanMember, clanMembers);
                        this.GetPlayerActivityData(table, ActivityModeType.AllPvE, clanMember, clanMembers);
                        this.GetPlayerActivityData(table, ActivityModeType.TrialsOfTheNine, clanMember, clanMembers);
                        this.GetPlayerActivityData(table, ActivityModeType.IronBanner, clanMember, clanMembers);
                    }
                    catch (Exception ex)
                    {
                        this.WriteMessage(ex.Message);
                    }
                }

                string defaultFileLocation = string.Format("{0}{1}.xlsx", AppDomain.CurrentDomain.BaseDirectory, "TestExport");

                FileInfo file = new FileInfo(defaultFileLocation);

                if (file.Exists)
                {
                    file.Delete();
                }

                using (ExcelPackage pkg = new ExcelPackage(file))
                {
                    ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Test");
                    bool includeHeaders = true;
                    ws.Cells["A1"].LoadFromDataTable(table, includeHeaders);
                    ws.View.FreezePanes(2, 1);
                    ws.Row(1).Style.Font.Bold = true;
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    pkg.Save();
                }

                this.WriteMessage("Done.");
            }
            catch (Exception ex)
            {
                this.WriteMessage(ex.Message);
            }
        }

        private void GetPlayerActivityData(DataTable table, ActivityModeType activityType, Player playerToCheck, List<Player> clanMembers)
        {
            string activityTypeName = Enum.GetName(typeof(ActivityModeType), activityType);
            this.WriteMessage(activityTypeName);

            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(playerToCheck.Id, playerToCheck.BungieMembershipType);
            int countOfActivitiesToGet = 10;

            int countOfActivities = 0;
            int countOfSoloActivities = 0;
            int countOfActivitiesWithClanMembers = 0;

            List<Activity> playerActivities = new List<Activity>();

            foreach (string characterId in characterIds)
            {
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    playerToCheck.BungieMembershipType,
                    playerToCheck.Id,
                    characterId,
                    countOfActivitiesToGet,
                    activityType);

                Thread.Sleep(100);

                playerActivities.AddRange(activities);
            }

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

            int countOfActivitieswithoutClanMembers = (countOfActivities - countOfActivitiesWithClanMembers - countOfSoloActivities);

            string earliestPeriod = string.Empty;

            if (playerActivities.Count > 0)
            {
                earliestPeriod = playerActivities[playerActivities.Count - 1].Period.ToString();
            }

            //DateTime startTime = playerActivities[playerActivities.Count - 1].Period;
            //DateTime endTime = playerActivities[0].Period;

            table.Rows.Add(new object[]
            {
                playerToCheck.Name,
                activityTypeName,
                countOfActivities,
                countOfActivitiesWithClanMembers,
                countOfSoloActivities,
                countOfActivitieswithoutClanMembers,
                earliestPeriod
            });
        }

        private void RunTestForActivitiesWithClan()
        {
            this.WriteMessage("Getting activity history...");
            DateTime methodStartTime = DateTime.Now;

            // Get the clan id.
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);

            // Get the list of members in the clan.
            List<Player> clanMembers = ClanInfo.GetMemberList(clanId);

            // Get the player info.
            string playerNameToGet = "psnPlayer";
            Player player = PlayerInfo.GetPsnPlayer(playerNameToGet);

            // Get the list of characters for the player.
            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(player.Id, player.BungieMembershipType);
            int countOfActivitiesToGet = 100;

            int countOfActivities = 0;
            int countOfSoloActivities = 0;
            int countOfActivitiesWithClanMembers = 0;

            List<Activity> playerActivities = new List<Activity>();

            foreach (string characterId in characterIds)
            {
                // Get the activities for a character, that fit the criteria.
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    player.BungieMembershipType,
                    player.Id,
                    characterId,
                    countOfActivitiesToGet,
                    ActivityModeType.Raid);

                playerActivities.AddRange(activities);
            }

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
                    if (playerInActivity.Name == playerNameToGet)
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

            DateTime startTime = playerActivities[playerActivities.Count - 1].Period;
            DateTime endTime = playerActivities[0].Period;

            this.WriteMessage("Activities: " + countOfActivities);
            this.WriteMessage("Activities with clan members: " + countOfActivitiesWithClanMembers);
            this.WriteMessage("Activities solo: " + countOfSoloActivities);
            this.WriteMessage("Activities without clan members: " + (countOfActivities - countOfActivitiesWithClanMembers - countOfSoloActivities));
            this.WriteMessage("Time start: " + startTime);
            this.WriteMessage("Time end: " + endTime);

            DateTime methodEndTime = DateTime.Now;
            TimeSpan methodDuration = methodEndTime - methodStartTime;
            this.WriteMessage(string.Format("Method duration: {0}:{1}:{2}", methodDuration.Hours, methodDuration.Minutes, methodDuration.Seconds));

            DataTable table = new DataTable();
            table.Columns.Add("Player");
            table.Columns.Add("Activity");
            table.Columns.Add("Number of Activities");
            table.Columns.Add("Number of Activities with Clan Members");
            table.Columns.Add("Number of Activities Solo");
            table.Columns.Add("Number of Activities without Clan Members");

            table.Rows.Add(new object[]
            {
                player.Name,
                "Raid",
                countOfActivities,
                countOfActivitiesWithClanMembers,
                countOfSoloActivities,
                (countOfActivities - countOfActivitiesWithClanMembers - countOfSoloActivities)
            });

            string defaultFileLocation = string.Format("{0}{1}.xlsx", AppDomain.CurrentDomain.BaseDirectory, "TestExport");

            FileInfo file = new FileInfo(defaultFileLocation);

            using (ExcelPackage pkg = new ExcelPackage(file))
            {
                ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Test");
                bool includeHeaders = true;
                ws.Cells["A1"].LoadFromDataTable(table, includeHeaders);
                ws.View.FreezePanes(2, 1);
                ws.Row(1).Style.Font.Bold = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                pkg.Save();
            }
        }

        private void RunTestForActivitiesWithClan2()
        {
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);
            List<Player> clanMembers = ClanInfo.GetMemberList(clanId);

            string playerNameToGet = "psnPlayer";

            Player player = PlayerInfo.GetPsnPlayer(playerNameToGet);

            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(player.Id, player.BungieMembershipType);
            int countOfActivitiesToGet = 20;

            int countOfActivities = 0;
            int countOfActivitiesWithClanMembers = 0;

            foreach (string characterId in characterIds)
            {
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    player.BungieMembershipType,
                    player.Id,
                    characterId,
                    countOfActivitiesToGet,
                    ActivityModeType.None);

                foreach (Activity activity in activities)
                {
                    countOfActivities++;

                    List<Player> playersInActivity = PlayerInfo.GetPlayersInActivity(activity.InstanceId);

                    foreach (Player playerInActivity in playersInActivity)
                    {
                        bool foundMatch = false;

                        foreach (Player clanMember in clanMembers)
                        {
                            if (playerInActivity.Id == clanMember.Id && playerInActivity.Name != playerNameToGet)
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
            }

            this.WriteMessage("Activities: " + countOfActivities);
            this.WriteMessage("Activities with clan members: " + countOfActivitiesWithClanMembers);
            this.WriteMessage("Activities without clan members: " + (countOfActivities - countOfActivitiesWithClanMembers));
        }

        private void RunPlayerListInActivityTest()
        {
            Player player = PlayerInfo.GetPsnPlayer("psnPlayer");

            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(player.Id, player.BungieMembershipType);

            foreach (string characterId in characterIds)
            {
                this.WriteMessage("Character: " + characterId + Environment.NewLine);

                int countOfActivities = 3;
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    player.BungieMembershipType,
                    player.Id,
                    characterId,
                    countOfActivities,
                    ActivityModeType.None);

                foreach (Activity activity in activities)
                {
                    this.WriteMessage("Players in activity:" + Environment.NewLine);

                    List<Player> players = PlayerInfo.GetPlayersInActivity(activity.InstanceId);

                    foreach (Player playerInActivity in players)
                    {
                        this.WriteMessage(playerInActivity.Name);
                    }

                    this.WriteMessage(string.Empty);
                }
            }
        }

        private void RunActivityHistoryTest()
        {
            Player player = PlayerInfo.GetPsnPlayer("psnPlayer");

            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(player.Id, player.BungieMembershipType);

            foreach (string characterId in characterIds)
            {
                this.WriteMessage("Character: " + characterId + Environment.NewLine);

                int countOfActivities = 3;
                List<Activity> activities = PlayerInfo.GetActivityHistory(
                    player.BungieMembershipType, 
                    player.Id, 
                    characterId, 
                    countOfActivities, 
                    ActivityModeType.None);

                foreach (Activity activity in activities)
                {
                    this.WriteMessage("Period = " + activity.Period);
                    this.WriteMessage("InstanceId = " + activity.InstanceId);
                    this.WriteMessage("Modes:");

                    foreach (ActivityModeType type in activity.Modes)
                    {
                        this.WriteMessage(type);
                    }

                    this.WriteMessage(string.Empty);
                }
            }
        }

        private void RunLeaderboardsTest2()
        {
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);
            List<LeaderboardSection> leaderboardSections = LeaderboardInfo.GetRaidLeaderboard(clanId, 10);

            this.WriteMessage("Raid Stats" + Environment.NewLine);

            foreach (LeaderboardSection leaderboardSection in leaderboardSections)
            {
                this.WriteMessage(leaderboardSection.Name + Environment.NewLine);

                foreach (LeaderboardEntry leaderEntry in leaderboardSection.LeaderboardEntries)
                {
                    this.WriteMessage(string.Format("{0}.) {1}, {2}", leaderEntry.Rank, leaderEntry.Player, leaderEntry.Value));
                }

                this.WriteMessage(string.Empty);
            }
        }

        private void RunLeaderboardsTest()
        {
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);
            List<LeaderboardSection> leaderboardSections = LeaderboardInfo.GetPrestigeNightfallLeaderboard(clanId, 10);

            this.WriteMessage("Prestige Nighfall Stats" + Environment.NewLine);

            foreach (LeaderboardSection leaderboardSection in leaderboardSections)
            {
                this.WriteMessage(leaderboardSection.Name + Environment.NewLine);

                foreach (LeaderboardEntry leaderEntry in leaderboardSection.LeaderboardEntries)
                {
                    this.WriteMessage(string.Format("{0}.) {1}, {2}", leaderEntry.Rank, leaderEntry.Player, leaderEntry.Value));
                }

                this.WriteMessage(string.Empty);
            }
        }

        private void TotalRaidCompletions()
        {
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);
            List<Player> clanMembers = ClanInfo.GetMemberList(clanId);

            this.WriteMessage("Raid completions:" + Environment.NewLine);

            foreach (Player member in clanMembers)
            {
                int raidsCompleted = StatsInfo.GetRaidCompletionsForAccount(member.BungieMembershipType, member.Id);
                this.WriteMessage(string.Format("{0} ({1}) = {2}", member.Name, member.BungieMembershipType, raidsCompleted));
            }
        }

        private void RunRaidTest()
        {
            Player player = PlayerInfo.GetPsnPlayer("psnPlayer");

            int raidsCompleted = StatsInfo.GetRaidCompletionsForAccount(MembershipType.TigerPsn, player.Id);

            this.WriteMessage(string.Format("Raids completed = {0}", raidsCompleted));
        }

        private void RunCharacterTest()
        {
            Player player = PlayerInfo.GetPsnPlayer("psnPlayer");

            List<string> characterIds = CharacterInfo.GetCharacterIdsForPlayer(player.Id, player.BungieMembershipType);

            foreach (string characterId in characterIds)
            {
                this.WriteMessage(characterId);
            }
        }

        private void RunPlayerTest()
        {
            Player player = PlayerInfo.GetPsnPlayer("psnPlayer");

            this.WriteMessage(string.Format("{0} {1} {2}", player.Name, player.Id, player.BungieMembershipType));
        }

        private void RunClanTest()
        {
            string clanName = "My Clan Name";
            string clanId = ClanInfo.GetClanId(clanName);
            List<Player> clanMembers = ClanInfo.GetMemberList(clanId);

            foreach (Player member in clanMembers)
            {
                this.WriteMessage(string.Format("{0} ({1})", member.Name, member.Id));
            }
        }

        private void WriteMessage(object message)
        {
            string formattedMessage = string.Format(
                "{0}: {1}{2}",
                DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss.fff"),
                message,
                Environment.NewLine);

            this.msgTextBox.Text += formattedMessage;
            this.msgTextBox.SelectionStart = this.msgTextBox.TextLength;
            this.msgTextBox.ScrollToCaret();
            this.msgTextBox.Update();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            this.msgTextBox.Clear();
            this.GetClanActivityMetrics();
        }
    }
}
