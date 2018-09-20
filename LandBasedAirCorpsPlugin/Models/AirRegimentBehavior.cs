using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandBasedAirCorpsPlugin.Models
{
    public enum AirRegimentBehavior
    {
        Standby,
        Sortie,
        Defense,
        Retreat,
        Rest
    }

    internal static class AirRegimentBehaviorExtensions
    {
        public static string GetText(this AirRegimentBehavior behavior)
        {
            switch (behavior)
            {
                case AirRegimentBehavior.Standby:
                    return "待機";
                case AirRegimentBehavior.Sortie:
                    return "出撃";
                case AirRegimentBehavior.Defense:
                    return "防空";
                case AirRegimentBehavior.Retreat:
                    return "退避";
                case AirRegimentBehavior.Rest:
                    return "休息";
                default:
                    return "";
            }
        }
    }

}
