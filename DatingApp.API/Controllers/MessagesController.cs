using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("/api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;
        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.datingRepository = datingRepository;
        }

        [HttpGet("{messageId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int messageId)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await datingRepository.GetMessage(messageId);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);           
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();  


            messageParams.UserId = userId;
            var messagesFromRepo = await datingRepository.GetMessagesForUser(messageParams);

            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
            messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();  

            var messagesFromRepo = await datingRepository.GetMessagesThread(userId, recipientId);

            var messagesThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messagesThread);            
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await datingRepository.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find recipient");

            var message = mapper.Map<Message>(messageForCreationDto);

            datingRepository.Add(message);

            if (await datingRepository.SaveAll())
            {
                var messageForReturn = mapper.Map<MessageForCreationDto>(message);
                return CreatedAtRoute("GetMessage", new {userId, messageId = message.Id}, messageForReturn);  
            }

            return BadRequest("Could not send Message");
                         
        }
        
    }
}