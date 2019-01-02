using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models
{
    public class RelocatingCompletedEventArgs : EventArgs
    {
        public string RegimentName { get; }

        public string MapAreaName { get; }

        public int SquadronId { get; }

        public string SlotItemName { get; }

        public int SlotItemId { get; }

        public RelocatingCompletedEventArgs(Squadron squadron, string regimentName, string mapAreaName)
        {
            this.RegimentName = regimentName;
            this.MapAreaName = mapAreaName;
            this.SquadronId = squadron.Id;
            this.SlotItemName = squadron.Name;
            this.SlotItemId = squadron.Plane.Id;
        }
    }
}
