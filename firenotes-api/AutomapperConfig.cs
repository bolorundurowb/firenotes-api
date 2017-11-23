/**
 * Created by winner-timothybolorunduro on 23/11/2017.
 */

using AutoMapper;
using firenotes_api.Models.Data;
using firenotes_api.Models.View;

namespace firenotes_api
 {
     public class AutomapperConfig : Profile
     {
         public AutomapperConfig()
         {
             CreateMap<User, AuthViewModel>();
         }
     }
 }