using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHDN.Models
{
    public class Post
    {
        public string id{get; set;}
        public User from { get; set; }
        public string message { get; set; }
        public string pictureUrl { get; set; }
        public int comment { get; set; }
        public int liked { get; set; }
        public string dateTime;

        public Post(string id, User from, string message, string pictureUrl, int comment, int liked, DateTime dateTime)
        {
            this.id = id;
            this.from = from;
            this.message = message;
            this.pictureUrl = pictureUrl;
            this.comment = comment;
            this.liked = liked;
            this.dateTime = dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        public Post(string id, User from, string message, string pictureUrl, int comment, int liked, string dateTime)
        {
            this.id = id;
            this.from = from;
            this.message = message;
            this.pictureUrl = pictureUrl;
            this.comment = comment;
            this.liked = liked;
            if (String.Compare(dateTime, "") == 0)
                this.dateTime = "";
            else
                this.dateTime = Post.convertDateTime(dateTime).ToString("dd/MM/yyyy HH:mm");
        }

        public static DateTime convertDateTime(string date)
        {
            if (String.Compare(date, "") == 0)
                return DateTime.Now;
            //string start = date.Substring(0, date.IndexOf("T"));
            //string end = date.Substring(date.IndexOf("T") + 1, date.IndexOf("+") - (date.IndexOf("T") + 1));
            //DateTime dt = Convert.ToDateTime(start +" "+ end);
            DateTime dt = DateTime.Parse(date);
            return dt;
        }
    }
}