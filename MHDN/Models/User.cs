using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHDN.Models
{
    public class User
    {
        public string name { get; set; }
        public string link { get; set; }
        public string pictureUrl { get; set; }
        public int comment { get; set; }
        public int liked { get; set; }
        public int post { get; set; }
        public int rate { get; set; }

        public User()
        {
            name = "";
            link = "";
            pictureUrl = "";
            comment = 0;
            liked = 0;
            post = 0;
        }
        public User(string name, string link, string pictureUrl, int comment, int liked, int post)
        {
            this.name = name;
            this.link = link;
            this.pictureUrl = pictureUrl;
            this.comment = comment;
            this.liked = liked;
            this.post = post;
            if (liked == 0)
                rate = 0;
            else
                rate = liked / (post + comment);
        }
    }
}