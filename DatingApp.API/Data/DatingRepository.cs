using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext dataContext;
        public DatingRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;

        }
        public void Add<T>(T entity) where T : class
        {
            dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            dataContext.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await dataContext.Photo.Where(userFromPhoto => userFromPhoto.UserId == userId)
                .FirstOrDefaultAsync(photo => photo.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await dataContext.Photo.FirstOrDefaultAsync(photo => photo.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await dataContext.Users.Include(user => user.Photos)
                .FirstOrDefaultAsync(user => user.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = dataContext.Users.Include(user => user.Photos)
            .OrderByDescending(user => user.LastActive).AsQueryable();
            
            users = users.Where(user =>  user.Id != userParams.UserId);
            users = users.Where(user => user.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(user => user.DateOfBirth >= minDateOfBirth && user.DateOfBirth <= maxDateOfBirth);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(user => user.Created);
                        break;

                    default:
                        users = users.OrderByDescending(user => user.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}