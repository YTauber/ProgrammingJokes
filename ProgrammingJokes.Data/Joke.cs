using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProgrammingJokes.Data
{
    public class Joke
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty("Id")]
        public int JokeId { get; set; }
        public string Setup { get; set; }
        public string Punchline { get; set; }

        public List<Like> Likes { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Like> Likes { get; set; }
    }

    public class Like
    {
        public int JokeId { get; set; }
        public int UserId { get; set; }
        public bool Liked { get; set; }
        public DateTime Time { get; set; }

        public Joke Joke { get; set; }
        public User User { get; set; }
    }

    public class LikeCount
    {
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
