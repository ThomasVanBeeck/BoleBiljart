using BoleBiljart.Interfaces;

namespace BoleBiljart.Models
{
    public class GlobalLookup : IModelHasKey
    {
        public string Key { get; set; } = null!;
        public string Uid { get; set; } = "singleton";
        //verboden of voorbehouden namen kan je hier aan toevoegen
         public Dictionary<string, int> Usernames { get; set; } = new Dictionary<string, int>() {
             { "admin",1 }, {"administrator",1 } };
    }
}