using System;
using System.Collections.Generic;

namespace DestinyHelper.Entity
{
    public class Activity
    {
        /// <summary>
        /// Gets or sets the Period property.
        /// </summary>
        public DateTime Period { get; set; }

        /// <summary>
        /// Gets or sets the InstanceId property.
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the Modes property.
        /// </summary>
        public List<ActivityModeType> Modes { get; set; }
    }
}
