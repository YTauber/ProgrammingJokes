using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ProgrammingJokes.Data
{
    public class JokeRepository
    {
        private string _connectionString;
        public JokeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Joke GetJoke()
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                using (HttpClient client = new HttpClient())
                {
                    string Json = client.GetStringAsync("https://official-joke-api.appspot.com/jokes/programming/random").Result;
                    Joke j = JsonConvert.DeserializeObject<IEnumerable<Joke>>(Json).FirstOrDefault();
                    if (context.Jokes.Any(jo => jo.JokeId == j.JokeId))
                    {
                        return context.Jokes.FirstOrDefault(jo => jo.JokeId == j.JokeId);
                    }
                    context.Jokes.Add(j);
                    context.SaveChanges();
                    return j;
                }
            }
        }

        public IEnumerable<Joke> GetAllJokes()
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                return context.Jokes.Include(j => j.Likes).ThenInclude(u => u.User).ToList();
            }
        }

        public Joke GetJokeByid(int id)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                return context.Jokes.FirstOrDefault(j => j.Id == id);
            }
        }

        public void AddUser(User user)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public User Verify(string email, string password)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                User user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return null;
                }
                bool good = BCrypt.Net.BCrypt.Verify(password, user.Password);

                if (!good)
                {
                    return null;
                }
                return user;
            }
        }

        public User GetUserByEmail(string email)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public LikeCount AddLike(int JokeId, int UserId, bool likes)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                Like like = context.Likes.FirstOrDefault(l => l.JokeId == JokeId && l.UserId == UserId);
                if (like == null)
                {
                    context.Likes.Add(new Like
                    {
                        UserId = UserId,
                        JokeId = JokeId,
                        Liked = likes,
                        Time = DateTime.Now
                    });
                }

                else
                {
                    if (like.Liked != likes && like.Time.AddMinutes(1) > DateTime.Now)
                    {
                        like.Liked = likes;
                        context.Likes.Attach(like);
                        context.Entry(like).State = EntityState.Modified;
                    }
                }
                context.SaveChanges();

                return new LikeCount
                {
                    Likes = context.Likes.Where(l => l.JokeId == JokeId && l.Liked == true).Count(),
                    Dislikes = context.Likes.Where(l => l.JokeId == JokeId && l.Liked == false).Count()
                };
            }
        }

        public LikeCount GetLikeCount(int JokeId)
        {
            using (JokeContext context = new JokeContext(_connectionString))
            {
                return new LikeCount
                {
                    Likes = context.Likes.Where(l => l.JokeId == JokeId && l.Liked == true).Count(),
                    Dislikes = context.Likes.Where(l => l.JokeId == JokeId && l.Liked == false).Count()
                };
            }
        }
    }
}
