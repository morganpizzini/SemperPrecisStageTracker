using DnetIndexedDb;
using Microsoft.JSInterop;

namespace SemperPrecisStageTracker.Blazor.Services.IndexDB
{
    public class MatchServiceIndexedDb : IndexedDbInterop
    {
        public MatchServiceIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<MatchServiceIndexedDb> options)
            : base(jsRuntime, options)
        {
        }
    }
}
