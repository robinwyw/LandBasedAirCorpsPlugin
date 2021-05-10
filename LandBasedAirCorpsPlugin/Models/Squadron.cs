using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using LandBasedAirCorpsPlugin.Models.Raw;
using Livet;

namespace LandBasedAirCorpsPlugin.Models
{
    public class Squadron : RawDataWrapper<kcsapi_plane_info>, IIdentifiable
    {
        public int Id => this.RawData.api_squadron_id;

        public string Name => this.State != SquadronState.Undeployed ? this.Plane.Info.Name : "未配備";

        public int Condition => this.RawData.api_cond;

        public int DeploymentCount => this.RawData.api_max_count;

        public int WorkingCount { get; internal set; }

        public SquadronState State { get; internal set; }

        public int Distance => this.Plane?.Info.RawData.api_distance ?? 0;

        #region Plane変更通知プロパティ
        private SlotItem _Plane;

        public SlotItem Plane
        {
            get { return this._Plane; }
            set
            { 
                if (this._Plane == value)
                    return;
                this._Plane = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public double AirSuperiorityAtSortie
        {
            get
            {
                if (this.State != SquadronState.Deployed) return 0;

                var info = this.Plane.Info;
                var intercept = this.IsInterceptor ? info.Evade : 0;
                var improvementBonus = this.GetImprovementBonus(this.Plane);
                var ex = info.Id == 138 && info.Name == "二式大艇" ?
                            this.WorkingCount >= 4 && this.Plane.Level >= 4 ? 1 :
                         info.Id == 312 && info.Name == "二式陸上偵察機(熟練)" ?
                            this.WorkingCount >= 4 && this.Plane.Level >= 2 ? 1 : 0
                         : 0 : 0;

                return Math.Floor((info.AA + (1.5 * intercept) + improvementBonus) * Math.Sqrt(this.WorkingCount) + this.Plane.GetBonus() + ex);
            }
        }

        public double AirSuperiorityAtDefense
        {
            get
            {
                if (this.State != SquadronState.Deployed) return 0;

                var info = this.Plane.Info;
                var intercept = this.IsInterceptor ? info.Evade : 0;
                var antiBomber = this.IsInterceptor ? info.Hit : 0;
                var improvementBonus = this.GetImprovementBonus(this.Plane);

                return Math.Floor((info.AA + intercept + (2 * antiBomber) + improvementBonus) * Math.Sqrt(this.WorkingCount) + this.Plane.GetBonus());
            }
        }

        public bool IsInterceptor => this.Plane?.Info.Type == SlotItemType.局地戦闘機;

        public Squadron(kcsapi_plane_info raw) : base(raw)
        {
            this.Plane = KanColleClient.Current.Homeport.Itemyard.SlotItems[raw.api_slotid];
            this.State = (SquadronState)raw.api_state;
            this.WorkingCount = raw.api_count;
        }

        private double GetImprovementBonus(SlotItem slotItem)
        {
            switch (slotItem.Info.Type)
            {
                case SlotItemType.艦上戦闘機:
                case SlotItemType.水上戦闘機:
                case SlotItemType.局地戦闘機:
                    return 0.2 * slotItem.Level;
                case SlotItemType.艦上爆撃機:
                    return (slotItem.Info.Id == 60 && slotItem.Info.Name == "零式艦戦62型(爆戦)") ||
                           (slotItem.Info.Id == 154 && slotItem.Info.Name == "零戦62型(爆戦/岩井隊)") ||
                           (slotItem.Info.Id == 219 && slotItem.Info.Name == "零式艦戦63型(爆戦)") ?
                                0.25 * slotItem.Level : 0;
                case SlotItemType.陸上攻撃機:
                case SlotItemType.大型陸上機:
                    return 0.5 * Math.Sqrt(slotItem.Level);
                default:
                    return 0;
            }            
        }

        public override string ToString()
        {
            return $"第{this.Id}中隊 : State = {this.State}, Plane = \"{this.Plane}\", {this.WorkingCount}/{this.DeploymentCount}";
        }
    }
}