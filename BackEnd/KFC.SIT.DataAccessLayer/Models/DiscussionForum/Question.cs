﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.DiscussionForum
{
    public class Question : IEntity 
    {
        public Question()
        {
            IsClosed = false;
            SpamCount = 0;
            Answers = new List<Answer>();
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public int PosterId { get; set; }

        public string PosterUserName { get; set; }

        // should only be 500 - 2000 chars
        public string Text { get; set; }

        public int MinimumExpForAnswer { get; set; }

        public bool IsDraft { get; set; }

        public bool IsClosed { get; set; }

        public int SpamCount { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
