using Firebase.Database;
using System.Reactive.Linq;
using Firebase.Database.Query;

namespace BoleBiljart.Services
{
    public class UserService : AbstractBaseService<Models.User>
    {
        public UserService(FirebaseClient fbClient) : base(fbClient)
        { }

        public IObservable<Models.User> GetByUsernameAsync(string username)
        {
            return _fbClient
                .Child(typeof(Models.User).Name)
                .AsObservable<Models.User>()
                .Where(x => x.Object != null && x.Object.Username == username)
                .Select(x =>
                {
                    x.Object!.Key = x.Key;
                    return x.Object;
                });
        }

        public IObservable<Models.User> GetByUidAsync(string uid)
        {
            return _fbClient
                .Child(typeof(Models.User).Name)
                .OrderBy("Uid")
                .EqualTo(uid)
                .AsObservable<Models.User>()
                .Where(x => x.Object != null && x.Object.Uid == uid)
                .Select(x =>
                {
                    x.Object!.Key = x.Key;
                    return x.Object;
                });
        }
    }
}