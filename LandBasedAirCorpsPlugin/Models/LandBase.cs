using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using LandBasedAirCorpsPlugin.Models.Settings;
using LandBasedAirCorpsPlugin.Models.Raw;
using Livet;

namespace LandBasedAirCorpsPlugin.Models
{
    public class LandBase : NotificationObject
    {
        private Notifier notifier;

        #region AirFleets変更通知プロパティ
        private MemberTable<AirFleet> _AirFleets;

        public MemberTable<AirFleet> AirFleets
        {
            get { return this._AirFleets; }
            set
            { 
                if (this._AirFleets == value)
                    return;
                this._AirFleets = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region RelocatingSquadrons 変更通知プロパティ

        private ObservableSynchronizedCollection<RelocatingSquadron> _RelocatingSquadrons;

        public ObservableSynchronizedCollection<RelocatingSquadron> RelocatingSquadrons
        {
            get
            { return this._RelocatingSquadrons; }
            set
            { 
                if (this._RelocatingSquadrons == value)
                    return;
                this._RelocatingSquadrons = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        public LandBase(Notifier notifier)
        {
            this.notifier = notifier;
            this.RelocatingSquadrons = new ObservableSynchronizedCollection<RelocatingSquadron>();

            var proxy = KanColleClient.Current.Proxy;

            proxy.ApiSessionSource
                .Where(x =>new Uri(x.HttpClient.Request.Url).PathAndQuery == "/kcsapi/api_get_member/mapinfo")
                .TryParse<kcsapi_mapinfo>()
                .Subscribe(x => this.Update(x.Data));
            proxy.ApiSessionSource
                .Where(x =>new Uri(x.HttpClient.Request.Url).PathAndQuery == "/kcsapi/api_req_air_corps/change_name")
                .TryParse()
                .Subscribe(this.ChangeName);
            proxy.ApiSessionSource
                .Where(x => new Uri(x.HttpClient.Request.Url).PathAndQuery == "/kcsapi/api_req_air_corps/set_action")
                .TryParse()
                .Subscribe(this.UpdateBehavior);
            proxy.ApiSessionSource
                .Where(x => new Uri(x.HttpClient.Request.Url).PathAndQuery == "/kcsapi/api_req_air_corps/supply")
                .TryParse()
                .Subscribe(this.Supply);
            proxy.ApiSessionSource
                .Where(x => new Uri(x.HttpClient.Request.Url).PathAndQuery == "/kcsapi/api_req_air_corps/set_plane")
                .TryParse<kcsapi_air_base>()
                .Subscribe(this.UpdateSquadrons);

            proxy.ApiSessionSource
                .SkipUntil(proxy.api_req_map_start.TryParse<kcsapi_map_start>().Do(x => this.Sortie(x.Data)))
                .TakeUntil(proxy.api_port)
                .Finally(this.Homing)
                .Repeat()
                .Subscribe();
#if DEBUG
            proxy.ApiSessionSource
                .Where(x => new Uri(x.HttpClient.Request.Url).PathAndQuery.StartsWith("/kcsapi/api_req_air_corps/"))
                .Subscribe(x =>
                {
                    string req = x.GetRequestBodyAsString().GetAwaiter().GetResult();
                    System.Diagnostics.Debug.WriteLine(Uri.UnescapeDataString(req));
                });
#endif
        }

        private void Update(kcsapi_mapinfo mapinfo)
        {
            var fleets = mapinfo.api_air_base
                .GroupBy(x => x.api_area_id, x => x)
                .Select(x => new AirFleet(x, x.Key));
            this.AirFleets = new MemberTable<AirFleet>(fleets);
        }

        private void ChangeName(SvData data)
        {
            if (data == null || !data.IsSuccess) return;

            try
            {
                var regiment = this.AirFleets[int.Parse(data.Request["api_area_id"])]
                                   .Regiments[int.Parse(data.Request["api_base_id"])];
                var newName = data.Request["api_name"];

                regiment.Name = newName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"基地航空隊の名前の変更に失敗しました。\n{ex}");
            }
        }

        private void UpdateBehavior(SvData data)
        {
            if (data == null || !data.IsSuccess) return;

            try
            {
                var fleet = this.AirFleets[int.Parse(data.Request["api_area_id"])];
                var regiments = data.Request["api_base_id"].Split(',');
                var behaviors = data.Request["api_action_kind"].Split(',');

                for (int i = 0; i < regiments.Length; i++)
                {
                    var regiment = fleet.Regiments[int.Parse(regiments[i])];
                    var behavior = (AirRegimentBehavior)int.Parse(behaviors[i]);

                    regiment.Behavior = behavior;
                }

                fleet.UpdateFleetState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"基地航空隊の挙動の変更に失敗しました。\n{ex}");
            }
        }

        private void Supply(SvData data)
        {
            if (data == null || !data.IsSuccess) return;

            try
            {
                var fleet = this.AirFleets[int.Parse(data.Request["api_area_id"])];
                var regiment = fleet.Regiments[int.Parse(data.Request["api_base_id"])];

                regiment.Supply(data.Request["api_squadron_id"].Split(','));
                fleet.UpdateFleetState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"航空機の補給に失敗しました。\n{ex}");
            }
        }

        private void UpdateSquadrons(SvData<kcsapi_air_base> data)
        {
            if (data == null || !data.IsSuccess) return;

            var raw = data.Data;
            try
            {
                var fleet = this.AirFleets[int.Parse(data.Request["api_area_id"])];
                var regiment = fleet.Regiments[int.Parse(data.Request["api_base_id"])];

                if (int.Parse(data.Request["api_item_id"]) == -1)
                {
                    var info = raw.api_plane_info[0];
                    var prev = regiment.UnsetSquadron(info.api_squadron_id, (SquadronState)info.api_state);

                    this.RelocateSquadron(prev);
                }
                else if (raw.api_plane_info.Length == 1)
                {
                    var info = raw.api_plane_info[0];
                    var prev = regiment.SetOrReplaceSquadron(info);

                    if (prev != null)
                        this.RelocateSquadron(prev);
                }
                else
                {
                    regiment.ExchangeSquadrons(raw.api_plane_info);
                }

                regiment.Distance = raw.api_distance.api_base + raw.api_distance.api_bonus;

                fleet.UpdateFleetState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"基地航空隊の航空機の変更に失敗しました。\n{ex}");
            }
        }

        private void RelocateSquadron(RelocatingSquadron squadron)
        {
            squadron.Completed += Handler;

            this.RelocatingSquadrons.Add(squadron);

            void Handler(object sender, RelocatingCompletedEventArgs e)
            {
                if (PluginSettings.NotifyRelocatingCompleted)
                {
                    var message = e.RegimentName == null
                        ? string.Format(Properties.Resources.RelocatingCompletedMessage, e.SlotItemName)
                        : string.Format(Properties.Resources.RelocatingCompleteMessageWithRegimentName, e.MapAreaName, e.RegimentName, e.SlotItemName);

                    this.notifier.Notify(
                        Notifier.Types.RelocatingComplete,
                        "配置転換完了",
                        message);
                }

                this.RelocatingSquadrons.Remove(squadron);
                squadron.Completed -= Handler;
            }
        }

        private void Sortie(kcsapi_map_start raw)
        {
            var fleet = this.AirFleets.Values.FirstOrDefault(x => x.MapArea.Id == raw.api_maparea_id);
            if (fleet == null || !fleet.CanSortie) return;

            fleet.Sortie();
        }

        private void Homing()
        {
            if (this.AirFleets == null) return;

            foreach (var fleet in this.AirFleets.Values)
            {
                fleet.Homing();
            }
        }
    }
}
