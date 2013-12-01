using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MHDN.Models;

namespace MHDN.Models
{
    public class Counting
    {
        public string id { get; set; }
        public int comment { get; set; }
        public int liked { get; set; }
        public int post { get; set; }
        public int rate { get; set; }
        public string createdTime { get; set; }
        public string name { get; set; }

        public Counting()
        {
            id = "";
            comment = 0;
            liked = 0;
            post = 0;
            rate = 0;
        }
        public Counting(string id, int comment, int liked, int post, string createdTime)
        {
            this.comment = comment;
            this.liked = liked;
            this.post = post;
            this.id = id;
            this.createdTime = Post.convertDateTime(createdTime).ToString();
            rate = 0;
        }

        public Counting(string id, int comment, int liked, int post, string createdTime, string name)
        {
            this.comment = comment;
            this.liked = liked;
            this.post = post;
            this.id = id;
            this.createdTime = Post.convertDateTime(createdTime).ToString();
            this.name = name;
            rate = 0;
        }
        public Counting(string id)
        {
            this.comment = 0;
            this.liked = 0;
            this.post = 0;
            this.id = id;
        }
        static public void countRate(List<Counting> l)
        {
            foreach (Counting e in l)
            {
                if (e.liked == 0)
                    e.rate = 0;
                else
                    e.rate = e.liked / (e.post + e.comment);
            }
        }
        public static int commentCompare(Counting sp1, Counting sp2)
        {
            if (sp1.comment == sp2.comment)
                return 0;
            if (sp1.comment > sp2.comment)
                return -1;
            return 1;
        }

        public static int likedCompare(Counting sp1, Counting sp2)
        {
            if (sp1.liked == sp2.liked)
                return 0;
            if (sp1.liked > sp2.liked)
                return -1;
            return 1;
        }

        public static int postCompare(Counting sp1, Counting sp2)
        {
            if (sp1.post == sp2.post)
                return 0;
            if (sp1.post > sp2.post)
                return -1;
            return 1;
        }

        public static int rateCompare(Counting sp1, Counting sp2)
        {
            if (sp1.rate == sp2.rate)
                return 0;
            if (sp1.rate > sp2.rate)
                return -1;
            return 1;
        }

        public static int createdTimeCompare(Counting sp1, Counting sp2)
        {
            if (String.Compare(sp1.createdTime, "") == 0 || String.Compare(sp2.createdTime, "") == 0)
                return 0;
            DateTime date1 = DateTime.ParseExact(sp1.createdTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(sp2.createdTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            return DateTime.Compare(date2, date1);
        }

    }
}