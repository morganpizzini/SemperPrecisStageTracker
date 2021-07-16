using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services.IndexDB;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Contracts;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class MainServiceLayer
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpService _httpService;
        private readonly MatchServiceIndexedDb _matchServiceIndexDb;
        private bool offline;
        public MainServiceLayer(ILocalStorageService localStorage,IHttpService httpService,MatchServiceIndexedDb matchServiceIndexDb)
        {
            _localStorage = localStorage;
            _httpService = httpService;
            _matchServiceIndexDb = matchServiceIndexDb;

            Task t3 = Task.Run( async () => {
                var model = await _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
                offline = model.OfflineMode;
                Console.WriteLine("pippo");
            });
            
            //var model = _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
            //model.RunSynchronously();
            //model.Wait();
            //if(model.IsCompleted)
            //{
            //    var t= model.Result;
            //}

        }

        public async Task UpdateModel(ClientSetting model)
        {
            await _localStorage.SetItem(CommonVariables.ClientSettingsKey,model);

            offline = model.OfflineMode;
            if (!model.OfflineMode || string.IsNullOrEmpty(model.MatchId))
                return;
            // download everything about model.MatchId
            
        }

        public async Task<IList<MatchContract>> GetMatches()
        {
            if (offline)
            {
                return await _matchServiceIndexDb.GetAll<MatchContract>("MatchContract");
            }
            return await _httpService.Post<IList<MatchContract>>("api/Match/FetchAllMatches");
        }
    }
}