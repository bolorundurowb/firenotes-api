/**
 * Created by winner-timothybolorunduro on 23/11/2017.
 */

using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using shortid;

namespace firenotes_api.Models
 {
     public class Note
     {
         [BsonId]
         public string Id { get; set; }

         public string Title { get; set; }

         public string Details { get; set; }

         public List<string> Tags { get; set; }

         public DateTime Created { get; set; }

         public bool IsFavorited { get; set; }

         public Note()
         {
             Id = ShortId.Generate(false, true, 10);
             Tags = new List<string>();
         }
     }
 }