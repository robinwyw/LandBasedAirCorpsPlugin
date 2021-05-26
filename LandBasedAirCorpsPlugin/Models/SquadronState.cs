using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models
{
    public enum SquadronState
    {
        /// <summary>
        /// 未配属。
        /// </summary>
        Undeployed,
        /// <summary>
        /// 配属済み。
        /// </summary>
        Deployed,
        /// <summary>
        /// 配置転換中。
        /// </summary>
        Relocating
    }
}
