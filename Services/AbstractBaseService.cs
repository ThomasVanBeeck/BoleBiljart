using BoleBiljart.Interfaces;
using Firebase.Database;
using Firebase.Database.Query;
using System.Reactive.Linq;

namespace BoleBiljart.Services
{
    public class AbstractBaseService<TModel> where TModel : class, IModelHasKey
    {
        protected readonly FirebaseClient _fbClient;

        public AbstractBaseService(FirebaseClient fbClient)
        {
            _fbClient = fbClient;
        }

        public async Task PostAsync(TModel model)
        {
            var returnedModel = await _fbClient
                .Child(typeof(TModel).Name)
                .PostAsync(model);
            // Key wordt gegenereerd door db extern, in memory het object updaten
            // met  geretourneerde key, handig om nadien Update te pushen naar db via Key
            model.Key = returnedModel.Key;
        }


        public IObservable<TModel> GetAllAsync()
        {
            return _fbClient
                .Child(typeof(TModel).Name)
                .AsObservable<TModel>()
                .Where(x => x.Object != null)
                .Select(x => {
                    x.Object!.Key = x.Key;
                    return x.Object;
                    }
                );
        }


        public async Task UpdateAsync(TModel model)
        {
            await _fbClient
                .Child(typeof(TModel).Name)
                .Child(model.Key)
                .PutAsync(model);
        }

        public async Task DeleteAsync(TModel model)
        {
            await _fbClient
                .Child(typeof(TModel).Name)
                .Child(model.Key)
                .DeleteAsync();
        }

        /* DEPRECATED
        public async Task PutUsingUidAsync(TModel model, string uid)
        {
            await _fbClient
            .Child(typeof(TModel).Name)
            .Child(uid)
            .PostAsync(model);
        }
        public IObservable<TModel> GetAllByUidAsync(string uid)
        {
            return _fbClient
                .Child(typeof(TModel).Name)
                .Child(uid)
                .AsObservable<TModel>()
                .Where(x => x.Object != null)
                .Select(x => x.Object!);
        }
        */
    }
}
