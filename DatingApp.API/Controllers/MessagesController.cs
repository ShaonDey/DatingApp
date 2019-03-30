using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            else
            {
                var messageFromRepo = await _repo.GetMessage(id);

                if (messageFromRepo == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(messageFromRepo);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            else
            {
                messageForCreationDto.SenderId = userId;

                var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

                if (recipient == null)
                {
                    return BadRequest("Could not find the user");
                }
                else
                {
                    var message = _mapper.Map<Message>(messageForCreationDto);

                    _repo.Add(message);

                    var messageToReturn = _mapper.Map<MessageForCreationDto>(message);

                    if (await _repo.SaveAll())
                    {
                        return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
                    }
                    else
                    {
                        throw new Exception("Creating the message failed");
                    }
                }
            }
        }
    }
}
