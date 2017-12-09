using System;

namespace DestinyHelper.Entity
{
    public class ActivityCount
    {
        /// <summary>
        /// Gets or sets the CountOfActivities property.
        /// </summary>
        public int CountOfActivities { get; set; }

        /// <summary>
        /// Gets or sets the CountOfSoloActivities property.
        /// </summary>
        public int CountOfSoloActivities { get; set; }

        /// <summary>
        /// Gets or sets the CountOfActivitiesWithClanMembers property.
        /// </summary>
        public int CountOfActivitiesWithClanMembers { get; set; }

        /// <summary>
        /// Gets the CountOfActivitieswithoutClanMembers property.
        /// </summary>
        public int CountOfActivitieswithoutClanMembers => (this.CountOfActivities - this.CountOfActivitiesWithClanMembers - this.CountOfSoloActivities);

        /// <summary>
        /// Gets the PercentPlayedWithClanMembers property.
        /// </summary>
        public double PercentPlayedWithClanMembers
        {
            get
            {
                double percentOfActivitiesWithClanMembers = 0;

                if (this.CountOfActivities > 0)
                {
                    percentOfActivitiesWithClanMembers = ((double)this.CountOfActivitiesWithClanMembers / (double)this.CountOfActivities) * 100;
                    percentOfActivitiesWithClanMembers = Math.Round(percentOfActivitiesWithClanMembers, 1);
                }

                return percentOfActivitiesWithClanMembers;
            }
        }
    }
}
