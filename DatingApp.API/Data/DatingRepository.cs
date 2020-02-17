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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await dataContext.Likes
            .FirstOrDefaultAsync(user => user.LikerId == userId && user.LikeeId == recipientId);
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

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(user => userLikers.Contains(user.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(user =>  userLikees.Contains(user.Id));
            }

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

        private async Task<IEnumerable<int>> GetUserLikes(int currentUserId, bool likers)
        {
            var user = await dataContext.Users.Include(x => x.Likers).Include(x => x.Likees)
                    .FirstOrDefaultAsync(user => user.Id == currentUserId);

            if (likers)
            {
                return user.Likers.Where(user => user.LikeeId == currentUserId).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(user => user.LikerId == currentUserId).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int messageId)
        {
            return await dataContext.Messages.FirstOrDefaultAsync(message => message.Id == messageId);
            
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = dataContext.Messages.Include(user => user.Sender).ThenInclude(userP => userP.Photos)
                            .Include(user => user.Recipient).ThenInclude(userP => userP.Photos).AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(userMess => userMess.RecipientId == messageParams.UserId);
                    break;
                
                case "Outbox":
                    messages = messages.Where(userMess => userMess.SenderId == messageParams.UserId);
                    break;

                default :
                    messages = messages.Where(userMess => userMess.RecipientId == messageParams.UserId && userMess.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(dateMess =>  dateMess.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            throw new NotImplementedException();
        }
    }
}