using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models.Raw
{
    public class kcsapi_plane_info
    {
        public int api_squadron_id { get; set; }
        public int api_state { get; set; }
        public int api_slotid { get; set; }
        public int api_count { get; set; }
        public int api_max_count { get; set; }
        public int api_cond { get; set; }
    }
}
