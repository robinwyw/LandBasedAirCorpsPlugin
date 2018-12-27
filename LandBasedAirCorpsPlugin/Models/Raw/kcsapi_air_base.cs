using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models.Raw
{
    public class kcsapi_air_base
    {
        public int api_area_id { get; set; }
        public int api_rid { get; set; }
        public string api_name { get; set; }
        public kcsapi_distance api_distance { get; set; }
        public int api_action_kind { get; set; }
        public kcsapi_plane_info[] api_plane_info { get; set; }
        public int api_after_bauxite { get; set; }
    }
}
