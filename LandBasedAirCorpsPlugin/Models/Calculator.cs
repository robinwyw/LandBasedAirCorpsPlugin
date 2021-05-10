using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace LandBasedAirCorpsPlugin.Models
{
    internal static class Calculator
    {
        private static readonly Dictionary<int, Proficiency> ProficiencyTable = new Dictionary<int, Proficiency>()
        {
            { 0, new Proficiency(0, 9, 0, 0) },
            { 1, new Proficiency(10, 24, 0, 0) },
            { 2, new Proficiency(25, 39, 2, 1) },
            { 3, new Proficiency(40, 54, 5, 1) },
            { 4, new Proficiency(55, 69, 9, 1) },
            { 5, new Proficiency(70, 84, 14, 3) },
            { 6, new Proficiency(85, 99, 14, 3) },
            { 7, new Proficiency(100, 120, 22, 6) },
        };

        private class Proficiency
        {
            public int InternalMinValue { get; }

            public int InternalMaxValue { get; }

            public int FighterBonus { get; }

            public int SeaplaneBomberBonus { get; }

            public Proficiency(int internalMin, int internalMax, int fighterBonus, int seaplaneBomberBonus)
            {
                this.InternalMinValue = internalMin;
                this.InternalMaxValue = internalMax;
                this.FighterBonus = fighterBonus;
                this.SeaplaneBomberBonus = seaplaneBomberBonus;
            }
        }

        public static double GetBonus(this SlotItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            var p = ProficiencyTable[item.Proficiency];

            switch (item.Info.Type)
            {
                case SlotItemType.艦上戦闘機:
                case SlotItemType.水上戦闘機:
                case SlotItemType.局地戦闘機:
                    return Math.Sqrt(p.InternalMinValue / 10) + p.FighterBonus;
                case SlotItemType.艦上爆撃機:
                case SlotItemType.艦上攻撃機:
                case SlotItemType.陸上攻撃機:
                case SlotItemType.水上偵察機:
                case SlotItemType.大型飛行艇:
                case SlotItemType.噴式戦闘爆撃機:
                case SlotItemType.艦上偵察機:
                    return Math.Sqrt(p.InternalMinValue / 10);
                case SlotItemType.水上爆撃機:
                    return Math.Sqrt(p.InternalMinValue / 10) + p.SeaplaneBomberBonus;
                default:
                    return 0;
            }
        }

        public static Squadron MaxByViewRange(this IEnumerable<Squadron> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Any(x => x.Plane.Info.Type == SlotItemType.艦上偵察機) ? source.Aggregate((x, y) => x.Plane.Info.ViewRange > y.Plane.Info.ViewRange ? x : y)
                 : source.Any(x => x.Plane.Info.Type == SlotItemType.陸上偵察機) ? source.Aggregate((x, y) => x.Plane.Info.ViewRange > y.Plane.Info.ViewRange ? x : y)
                 : source.Any(x => x.Plane.Info.Type == SlotItemType.大型飛行艇 ||
                                   x.Plane.Info.Type == SlotItemType.水上偵察機) ? source.Aggregate((x, y) => x.Plane.Info.ViewRange > y.Plane.Info.ViewRange ? x : y)
                 : null;
        }

        public static double GetSurveillanceBonus(this Squadron squadron)
        {
            if (squadron == null) return 1;

            var info = squadron.Plane.Info;
            if (info.Type == SlotItemType.艦上偵察機)
            {
                return info.ViewRange <= 7 ? 1.2 : 1.3;
            }
            else if(info.Type == SlotItemType.大型飛行艇 || info.Type == SlotItemType.水上偵察機)
            {
                return info.ViewRange <= 7 ? 1.1
                 : info.ViewRange == 8 ? 1.13
                 : 1.16;
            }
            else
            {
                return 1.18;
            }
            
        }
    }
}
