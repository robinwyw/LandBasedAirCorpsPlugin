using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models
{
    [Flags]
    public enum AirFleetSituation
    {
        Empty         = 0b0000_0000,
        Standby       = 0b0000_0001,
        Sortie        = 0b0000_0010,
        Defense       = 0b0000_0100,
        Relocating    = 0b0000_1000,
        InShortSupply = 0b0001_0000,
    }
}
