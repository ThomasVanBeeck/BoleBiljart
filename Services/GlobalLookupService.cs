using BoleBiljart.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Reactive.Linq;

namespace BoleBiljart.Services
{
    public class GlobalLookupService : AbstractBaseService<GlobalLookup>
    {
        public GlobalLookupService(FirebaseClient fbClient) : base(fbClient) { }

        public IObservable<Models.GlobalLookup> GetByUidAsync(string uid)
        {
            return _fbClient
                .Child(typeof(Models.GlobalLookup).Name)
                .OrderBy("Uid")
                .EqualTo(uid)
                .AsObservable<Models.GlobalLookup>()
                .Where(x => x.Object != null)
                .Select(x =>
                {
                    x.Object!.Key = x.Key;
                    return x.Object;
                });
        }
    }
}
