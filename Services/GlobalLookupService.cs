using BoleBiljart.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Reactive.Linq;

namespace BoleBiljart.Services
{
    public class GlobalLookupService : AbstractBaseService<GlobalLookup>
    {
        private readonly Dictionary<string, IObservable<GlobalLookup>> _cache = new();

        public GlobalLookupService(FirebaseClient fbClient) : base(fbClient) {
        }

        public IObservable<GlobalLookup> GetByUidAsync(string uid)
        {
            if (_cache.TryGetValue(uid, out var cachedObservable))
            {
                return cachedObservable;
            }

            var observable = _fbClient
                .Child(typeof(GlobalLookup).Name)
                .OrderBy("Uid")
                .EqualTo(uid)
                .AsObservable<GlobalLookup>()
                .Where(x => x.Object != null)
                .Select(x =>
                {
                    x.Object!.Key = x.Key;
                    return x.Object;
                })
                .Replay(1)
                .RefCount();

            _cache[uid] = observable;

            return observable;
        }
    }
}
