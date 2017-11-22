/**
 * Created by winner-timothybolorunduro on 22/11/2017.
 */

using shortid;

namespace firenotes_api.Models
 {
     public class User
     {
         public string Id { get; set; }
         
         public string Username { get; set; }
         
         public string HashedPassword { get; private set; }

         public string Password
         {
             set
             {
                 var salt = BCrypt.Net.BCrypt.GenerateSalt();
                 HashedPassword = BCrypt.Net.BCrypt.HashPassword(value, salt);
             }
         }
     }
 }