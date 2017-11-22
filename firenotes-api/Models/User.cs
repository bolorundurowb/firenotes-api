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
         
         public string HashedPassword { get; }

         public string Password
         {
             set
             {
                 
             }
         }
     }
 }