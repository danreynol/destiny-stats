using System.Collections.Generic;

namespace DestinyHelper.Entity
{
    public class LeaderboardSection
    {
        /// <summary>
        /// Gets or sets the Name property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the LeaderboardEntries property.
        /// </summary>
        public List<LeaderboardEntry> LeaderboardEntries { get; set; }
    }
}
