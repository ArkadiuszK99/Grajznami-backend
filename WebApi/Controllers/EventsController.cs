using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTO;

namespace WebApi.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventUsersService _eventUsersService;

        public EventsController(IEventService eventService, IEventUsersService eventUsersService)
        {
            _eventService = eventService;
            _eventUsersService = eventUsersService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableEvents()
        {
            var events = await _eventService.GetAvailableEvents();
            return Ok(events);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}")]
        public async Task<IActionResult> GetEventById([FromRoute] int id)
        {
            var foundEvent = await _eventService.GetEventById(id);

            if (foundEvent == null)
                return BadRequest();
            else
                return Ok(foundEvent);
        }

        //event z imieniem organizatora
        [HttpGet]
        [Route("/withorganiser/{id}")]
        public async Task<IActionResult> GetEventByIdWithOrganiserName([FromRoute] int id)
        {
            var foundEvent = await _eventService.GetEventWithUsername(id);
            foundEvent.OrganiserName = _eventUsersService.GetCurrentUser().FirstName;
            if (foundEvent == null)
                return BadRequest();
            else
                return Ok(foundEvent);
        }

        [HttpGet]
        [Route("/api/{eventId}")]
        public IActionResult GetEventUsers([FromRoute] int eventId)
        {
            var usersView = _eventUsersService.GetEventUsers(eventId);
            if (usersView == null)
                return BadRequest();
            else
                return Ok(usersView);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddEventDTO @event)
        {
            if (@event == null)
            {
                return BadRequest();
            }
            if (await _eventService.AddEvent(@event))
                if (await _eventService.SaveChangesAsync())
                {
                    string organiserId = _eventUsersService.GetCurrentUser().Id;
                    int thisEventId = _eventService.GetLastEvent(organiserId).Id;
                    _eventUsersService.SignCurrentUserToEvent(thisEventId);
                    return NoContent();
                }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (_eventService.DeleteEvent(id))
                if (await _eventService.SaveChangesAsync())
                    return NoContent();
            return NotFound();
        }

        [HttpPatch]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put([FromBody] ModifyEventDTO @event, [FromRoute] int id)
        {
            if (@event == null)
                return BadRequest();

            if (_eventService.ModifyEvent(@event, id)) 
                if (await _eventService.SaveChangesAsync())
                    return Ok();

            return BadRequest();
        }

    }
}
