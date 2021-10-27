using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILoggingService _loggingService;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventUsersService _eventUsersService;
        private readonly IInviteService _inviteService;
        private readonly ITrainersService _trainersService;

        public UserController(ILoggingService loggingService, IJwtAuthenticationManager jwtAuthenticationManager,
            ICurrentUserService currentUserService, IEventUsersService eventUsersService, ITokenRefresher tokenRefresher,
            IInviteService inviteService, ITrainersService trainersService)
        {
            _loggingService = loggingService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _currentUserService = currentUserService;
            _eventUsersService = eventUsersService;
            _tokenRefresher = tokenRefresher;
            _inviteService = inviteService;
            _trainersService = trainersService;
        }


        [HttpPost]
        [Route("{eventId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SignToEvent([FromRoute] int eventId)
        {
            var result = _eventUsersService.SignCurrentUserToEvent(eventId);
            if (result)
                return NoContent();
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("invite/{eventId}")]
        public IActionResult InviteToEvent([FromBody] EmailDTO model, [FromRoute] int eventId)
        {
            var result = _eventUsersService.InviteCurrentUserToEvent(eventId, model.Email);
            if (result)
                return NoContent();
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (await _loggingService.Login(model))
            {
                var jwtResponse = _jwtAuthenticationManager.Authenticate(model);
                return Ok(jwtResponse);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {

            if (await _loggingService.Register(model))
                return Ok();
            return BadRequest();
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] RefreshCred refreshCred)
        {
            var jwtResponse = _tokenRefresher.Refresh(refreshCred);

            if (jwtResponse == null)
                return Unauthorized();

            return Ok(jwtResponse);
        }

        [HttpDelete]
        [Route("{eventId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SignOutFromfEvent([FromRoute] int eventId)
        {
            var result = _eventUsersService.SignOutCurrentUserFromEvent(eventId);
            if (result)
                return NoContent();
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("showUsersToInvite/{eventId}")]
        public IActionResult ShowUsersToInvite([FromRoute] int eventId)
        {
            return Ok(_inviteService.GetUsersToInvite(eventId));
        }

        [HttpGet]
        [Route("returnInvitations")]
        public async Task<IActionResult> ReturnInvitations()
        {
            var events = await _inviteService.GetInvitations();
            return Ok(events);
        }

        [HttpGet]
        [Route("returnEventsUserIsSignedTo")]
        public async Task<IActionResult> ReturnEventsUserIsSignedTo()
        {
            var events = await _eventUsersService.GetEventsUserIsSignedTo();
            return Ok(events);
        }

        [HttpGet]
        [Route("userEvents")]
        public async Task<IActionResult> ReturnUserEvents()
        {
            var events = await _eventUsersService.GetUserEvents();
            return Ok(events);
        }

        [HttpGet]
        [Route("trainers/{sportName}")]
        public async Task<IActionResult> ReturnTrainers([FromRoute] string sportName)
        {
            var trainers = await _trainersService.GetTrainers(sportName);
            return Ok(trainers);
        }
    }
}
