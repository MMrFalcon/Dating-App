using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("/api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> options;
        private Cloudinary cloudinary;

        public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> options)
        {
            this.options = options;
            this.mapper = mapper;
            this.datingRepository = datingRepository;

            Account cloudinaryAccount = new Account (
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(cloudinaryAccount);

        }

        [HttpGet("{photoId}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int photoId) 
        {

            var photoFromRepo = await datingRepository.GetPhoto(photoId);
            var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);   
            
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto) 
        {
 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await datingRepository.GetUser(userId);   

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) {

                using (var stream = file.OpenReadStream()) 
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();   
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(user => user.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);
            
            if (await datingRepository.SaveAll())
            {
                var photoForReturn = mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {userId = userId, photoId = photo.Id}, photoForReturn);
                
            }

            return BadRequest("Could not add the photo");     
        }

        [HttpPost("{photoId}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await datingRepository.GetUser(userId);

            if (!user.Photos.Any(photo => photo.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await datingRepository.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await datingRepository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await datingRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId, int userId) 
        {
            
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await datingRepository.GetUser(userId);

            if (!user.Photos.Any(photo => photo.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await datingRepository.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo.");  

            if (photoFromRepo.PublicId != null)
            {
                var deletParams = new DeletionParams(photoFromRepo.PublicId);
                var result = cloudinary.Destroy(deletParams);

                if (result.Result == "ok")
                {
                    datingRepository.Delete(photoFromRepo);
                }
            } 
            else
            {
               datingRepository.Delete(photoFromRepo); 
            }
            

            if (await datingRepository.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo.");        
        }
    }
}