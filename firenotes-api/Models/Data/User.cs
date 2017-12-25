/**
 * Created by winner-timothybolorunduro on 22/11/2017.
 */

using MongoDB.Bson.Serialization.Attributes;
using shortid;

namespace firenotes_api.Models.Data
 {
     public class User
     {
         [BsonId]
         public string Id { get; set; }
         
         public string FirstName { get; set; }

         public string LastName { get; set; }
         
         public string Email { get; set; }
         
         public string HashedPassword { get; private set; }

         public bool IsArchived { get; set; }

         public string Password
         {
             set
             {
                 var salt = BCrypt.Net.BCrypt.GenerateSalt();
                 HashedPassword = BCrypt.Net.BCrypt.HashPassword(value, salt);
             }
         }

         public User()
         {
             Id = ShortId.Generate(false, true, 10);
             IsArchived = false;
         }
     }
 }