using ProgrammingJokes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammingJokes.Web.Models
{
    public class IndexViewModel
    {
        public Joke Joke { get; set; }
        public User CurrentUser { get; set; }
        public LikeCount LikeCount { get; set; }
    }

    public class JokesViewModel
    {
        public IEnumerable<Joke> Jokes { get; set; }
        public User CurrentUser { get; set; }
    }

}
