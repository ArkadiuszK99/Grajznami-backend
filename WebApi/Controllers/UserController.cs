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

        public UserController(ILoggingService loggingService, IJwtAuthenticationManager jwtAuthenticationManager,
            ICurrentUserService currentUserService, IEventUsersService eventUsersService, ITokenRefresher tokenRefresher, IInviteService inviteService)
        {
            _loggingService = loggingService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _currentUserService = currentUserService;
            _eventUsersService = eventUsersService;
            _tokenRefresher = tokenRefresher;
            _inviteService = inviteService;
        }

        [HttpGet]
        [Route("userevents")]
        public IActionResult GetUserEvents()
        {
            return Ok(_eventUsersService.GetUserEvents());
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
    }
}
