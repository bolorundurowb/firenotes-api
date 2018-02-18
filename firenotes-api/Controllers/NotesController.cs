using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using firenotes_api.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class NotesController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;

        public NotesController(IMapper mapper, INoteService noteService)
        {
            _mapper = mapper;
            _noteService = noteService;
        }

        // GET api/notes
        [Route(""), HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] NoteQueryModel query)
        {
            var callerId = GetIdFromClaims();
            
            var notes = await _noteService.GetNotes(callerId, query);
            return Ok(_mapper.Map<List<NoteViewModel>>(notes));
        }

        // GET api/notes/:id
        [Route("{id}"), HttpGet]
        public async Task<IActionResult> GetOne(string id)
        {
            var callerId = GetIdFromClaims();
            
            var note = await _noteService.GetNote(id, callerId);

            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        // POST api/notes
        [Route(""), HttpPost]
        public async Task<IActionResult> Create([FromBody] NoteBindingModel data)
        {
            var callerId = GetIdFromClaims();
            
            if (data == null)
            {
                return BadRequest("The payload must not be null.");
            }

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                return BadRequest("A title is required.");
            }

            var note = new Note
            {
                Owner = callerId,
                Title = data.Title,
                Details = data.Details,
                Tags = data.Tags,
                Created = DateTime.Now,
                IsFavorited = false
            };
            await _noteService.Add(note);

            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        // PUT api/notes/:id
        [Route("{id}"), HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] NoteBindingModel data)
        {
            var callerId = GetIdFromClaims();
            
            var note = await _noteService.GetNote(id, callerId);
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            if (data == null)
            {
                return Ok(_mapper.Map<NoteViewModel>(note));
            }

            await _noteService.Update(id, callerId, data);
            note = await _noteService.GetNote(id, callerId);

            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        // DELETE api/notes/:id
        [Route("{id}"), HttpDelete]
        public async Task<IActionResult> Remove(string id)
        {
            var callerId = GetIdFromClaims();
            
            await _noteService.Delete(id, callerId);


            return Ok("Note successfully removed.");
        }

        // POST api/notes/:id/favorite
        [Route("{id}/favorite"), HttpPost]
        public async Task<IActionResult> Favorite(string id)
        {
            var callerId = GetIdFromClaims();
            
            var note = await _noteService.GetNote(id, callerId);
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            await _noteService.SetFavorite(id, callerId);
            note = await _noteService.GetNote(id, callerId);

            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        // POST api/notes/:id/unfavorite
        [Route("{id}/unfavorite"), HttpPost]
        public async Task<IActionResult> UnFavorite(string id)
        {
            var callerId = GetIdFromClaims();
            
            var note = await _noteService.GetNote(id, callerId);
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            await _noteService.SetUnFavorite(id, callerId);
            note = await _noteService.GetNote(id, callerId);

            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        private string GetIdFromClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity?.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}