using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MHDN.Models;

namespace MHDN.Controllers
{
    public class HomeController : Controller
    {
        static string groupId = "405830319517708";
        static string appId = "430464820399215";
        static string appSecret = "bb662c9eaaef5f5da91b2413016361e9";
        static string accessToken = "";
        static dynamic allMember;
        static dynamic result;
        static List<Counting> postList;
        static List<Counting> members;
        static string startDate = "";
        static string endDate = "";
        private void getData()
        {
            var fbClient = new Facebook.FacebookClient();
            if (String.Compare(accessToken, "") == 0)
            { 
                result = fbClient.Get("oauth/access_token", new
                {
                    client_id = appId,
                    client_secret = appSecret,
                    grant_type = "client_credentials"
                });
                accessToken = result.access_token;
            }
            fbClient.AccessToken = accessToken;
            result = fbClient.Get(groupId + "/feed?limit=10000");//id of group
            //need user limit and offse

            bool isContinute = true;
            int limit = 500;
            //for (int i = 0; isContinute == true; i = i + limit)
            //{
            //    allMember = fbClient.Get(groupId + "/members?fields=name,id,link,picture&limit=" + limit + "&offset=" + i);
            //    isContinute = false;
            //}

            //members = getUserList(result);

            //postList = getPostList(result);
        }

        public ActionResult Index(FormCollection form)
        {
            ViewBag.Title = "POST";
            startDate = "";
            endDate = "";

            if (form["time"] != null)
            {
                if (String.Compare(form["time"].ToString(), "span") == 0)
                {
                    startDate = Request["startCalendar"].ToString();
                    endDate = Request["endCalendar"].ToString();
                }
            }
            string message = "";
            if (Request["time"] != null)
            {
                if (String.Compare(Request["time"].ToString(), "span") == 0)
                {
                    startDate = Request["startCalendar"].ToString();
                    endDate = Request["endCalendar"].ToString();
                    message = message + "(" + startDate + " - " + endDate + ")";
                }
                else
                    message = message + "(All time)";
            }
            else
                message = message + "(All time)";
            
           
            if (String.Compare(accessToken, "") == 0)
                getData();
 
            if (members == null)
                members = getUserList(result);

            //if (postList == null)
                postList = getPostList(result);

            int topNum = 10;
            if (Request["type"] != null)
            {
                if (String.Compare(Request["type"].ToString(), "comment") == 0)
                {
                    postList.Sort(Counting.commentCompare);
                    message = "Sort by comment" + message;
                }
                else
                {
                    postList.Sort(Counting.likedCompare);
                    message = "Sort by liked" + message;
                }
            }
            else
            {
                postList.Sort(Counting.likedCompare);
                message = "Sort by liked" + message;
            }
            ViewBag.Message = message;

            List<Post> topPost = getTopPost(postList, topNum, 0);

            ViewData["accessToken"] = accessToken;
            ViewData["topPost"] = topPost;
            ViewData["result"] = result;
            return View();
     }

        public ActionResult User(FormCollection form)
        {

            ViewBag.Title = "USER";
            startDate = "";
            endDate = "";

            if (form["time"] != null)
            {
                if (String.Compare(form["time"].ToString(), "span") == 0)
                {
                    startDate = Request["startCalendar"].ToString();
                    endDate = Request["endCalendar"].ToString();
                }
            }

            string message = "";
            if (Request["time"] != null)
            {
                if (String.Compare(Request["time"].ToString(), "span") == 0)
                {
                    startDate = Request["startCalendar"].ToString();
                    endDate = Request["endCalendar"].ToString();
                    message = message + "(" + startDate + " - " + endDate + ")";
                }
                else
                    message = "(All time)";
            }
            else
                message = "(All time)";


            if (String.Compare(accessToken, "") == 0)
                getData();

            //if (members == null)
            //    members = getUserList(result);

            //if (postList == null)
            members = getUserList(result);

            int topNum = 10;
            if (Request["type"] != null)
            {
                if (String.Compare(Request["type"].ToString(), "comment") == 0)
                {
                    members.Sort(Counting.commentCompare);
                    message = "Sort by comment " + message;
                }
                else
                {
                    if (String.Compare(Request["type"].ToString(), "liked") == 0)
                    {
                        members.Sort(Counting.likedCompare);
                        message = "Sort by liked " + message; 
                    }
                    else
                        if (String.Compare(Request["type"].ToString(), "post") == 0)
                        {
                            members.Sort(Counting.postCompare);
                            message = "Sort by liked " + message;
                        }
                        else
                            {
                                Counting.countRate(members);
                                members.Sort(Counting.rateCompare);
                                message = "Sort by rate: liked / (post + comment) " + message;
                            }
                }
            }
            else
            {
                members.Sort(Counting.likedCompare);
                message = "Sort by liked" + message;
            }

            ViewBag.Message = message;

            List<User> topMember = getTopUser(members, topNum);

            ViewData["accessToken"] = accessToken;
            ViewData["topMember"] = topMember;
            ViewData["result"] = result;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public JsonResult GetNextPost(int start)
        {
            if (String.Compare(accessToken, "") == 0)
                getData();
            if (postList == null)
                postList = getPostList(result);

            List<Post> topPost = null;
            if (start <= postList.Count)
            {
                int topNum = 10;
                //postList.Sort(Counting.likedCompare);
                topPost = getTopPost(postList, topNum, start);
            }
            return Json(topPost, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetComment(String postID)
        {
            if (String.Compare(accessToken, "") == 0 || postList == null)
                getData();

            List<Post> cms = new List<Post>();
            
            bool isContinute = true;

            for (int i = 0; i < result.data.Count && isContinute == true; i++)
            {
                if (String.Compare(result.data[i].id, postID) == 0)
                {
                    if (result.data[i].comments != null && result.data[i].comments.data.Count != 0)
                    {
                        foreach (var cm in result.data[i].comments.data)
                        {
                            User user = createUser(cm.from.id, cm.from.name, 0, 0, 0);
                            Post post = new Post(cm.id, user, cm.message, "", 0, (int)cm.like_count, cm.created_time);
                            cms.Add(post);
                        }
                        if (result.data[i].comments.data.Count == 25)
                        {
                            var fbClient = new Facebook.FacebookClient();
                            if (String.Compare(accessToken, "") == 0)
                            {
                                result = fbClient.Get("oauth/access_token", new
                                {
                                    client_id = appId,
                                    client_secret = appSecret,
                                    grant_type = "client_credentials"
                                });

                                //fbClient.AccessToken = result.access_token;
                                accessToken = result.access_token;
                            }
                            fbClient.AccessToken = accessToken;
                           // result = fbClient.Get(postID + "/comments?limit=100");//id of group
                            //need user limit and offse

                            bool isCon = true;
                            int limit = 70;
                            for (int j = 25; isCon == true; j = j + limit)
                            {
                                dynamic tmp = fbClient.Get(postID + "/comments?limit=" + limit + "&offset=" + j);

                                foreach (var cm in tmp.data)
                                {
                                    User user = createUser(cm.from.id, cm.from.name, 0, 0, 0);
                                    Post post = new Post(cm.id, user, cm.message, "", 0, (int)cm.like_count, cm.created_time);
                                    cms.Add(post);
                                }
                                if (tmp.data.Count == 0)
                                    isCon = false;
                            }

                        }
                        isContinute = false;
                    }
                }

            }
            return Json(cms, JsonRequestBehavior.AllowGet);
        }

        private User createUser(string id, string name, int comment, int liked, int post)
        {
            //var fbClient = new Facebook.FacebookClient();
            //fbClient.AccessToken = result.access_token;

            //string rq = id + "?fields=picture,link,name";
            //result = fbClient.Get(rq);//id of group

            //string name = result.name, link = result.link;
            ////rq = id + "?fields=picture";
            ////result = fbClient.Get(rq);
            //string url = result.picture.data.url;
            User tmp = null;
            //bool isContinute = true;
            tmp = new User(name, "https://www.facebook.com/" + id, "https://graph.facebook.com/" + id + "/picture", comment, liked, post);
            //foreach (var member in allMember.data)
            //{
            //    if (string.Compare(member.id, id) == 0)
            //    {
            //        tmp = new User(member.name, member.link, member.picture.data.url, comment, liked, post);
            //        break;
            //    }
            //}
            return tmp;
        }

        private List<User> getTopUser(List<Counting> countingList, int topNum)
        {
            List<User> topMembers = new List<User>();
            var fbClient = new Facebook.FacebookClient();
            fbClient.AccessToken = result.access_token;

            for (int i = 0; i < topNum && i < countingList.Count; i++)
            {
                int index = countingList.Count - i - 1;
                User tmp = createUser(countingList[i].id, countingList[i].name, countingList[i].comment, countingList[i].liked, countingList[i].post);
                //string rq = countingList[index].id;
                //result = fbClient.Get(rq);//id of group

                //string name = result.name, link = result.link;
                //rq = countingList[index].id + "?fields=picture";
                //result = fbClient.Get(rq);
                //string url = result.picture.data.url;
                //User tmp = new User(name, link, url, countingList[index].comment, countingList[index].liked, countingList[index].post);
                //tmp.Picture.Data = me.picture.data.url;
                topMembers.Add(tmp);
            }
            return topMembers;
        }

        private List<Post> getTopPost(List<Counting> counting, int topNum, int start)
        {
            List<Post> topPost = new List<Post>();
            var fbClient = new Facebook.FacebookClient();
            fbClient.AccessToken = result.access_token;
            for (int i = start; i < topNum + start && i < counting.Count; i++)
            {
               // int index = counting.Count - i - 1;
                string rq = "https://graph.facebook.com/" + counting[i].id;
                var resultTmp = result.data[0];
                foreach(var e in result.data)
                {
                    if (String.Compare(e.id, counting[i].id) == 0)
                    {
                        resultTmp = e;
                        break;
                    }
                }
                //result = fbClient.Get(rq);

                //string name = result.name, link = result.link;
                //rq = counting[index].id + "?fields=picture";
                //result = fbClient.Get(rq);
                //string url = result.picture.data.url;
                User from = createUser(resultTmp.from.id, resultTmp.from.name, 0, 0, 0);
                Post tmp = new Post((string)resultTmp.id, from, (string)resultTmp.message, (string)resultTmp.picture, counting[i].comment, counting[i].liked, resultTmp.created_time);
                //tmp.Picture.Data = me.picture.data.url;
                topPost.Add(tmp);
            }
            return topPost;
        }
        private List<Counting> getUserPostList(dynamic result, ref List<Counting> countingArr)
        {
            if (countingArr == null)
                countingArr = new List<Counting>();

            // dynamic result1 = fbClient.Get("fql", new { q = "SELECT uid FROM user WHERE uid=me()" });

            // while (me.data != null)
            // {
            foreach (var element in result.data)
            {
                if (isGetByDate(element.created_time) == true)
                {
                bool isExit = false;
                for (int i = 0; i < countingArr.Count && isExit == false; i++)
                {
                    //UserPost userPost in members
                    if (String.Compare(countingArr[i].id, element.from.id) == 0)
                    {
                        countingArr[i].post = countingArr[i].post + 1;
                        isExit = true;
                        // break;
                    }
                }
                if (isExit == false)
                {
                    Counting tmp = new Counting(element.from.id, 0, 0, 1, "");
                    countingArr.Add(tmp);
                }
            }
            }
            // me = fbClient.Get(me.paging.next);
            //}

            return countingArr;
   
        }

        private List<Counting> getUserCommentList(dynamic result, ref List<Counting> countingArr)
        {
            if (countingArr == null)
                countingArr = new List<Counting>();

            // dynamic result1 = fbClient.Get("fql", new { q = "SELECT uid FROM user WHERE uid=me()" });

            // while (me.data != null)
            // {
            foreach (var dataNode in result.data)
            {
                if (dataNode.comments != null)
                {
                    if (dataNode.comments.Count < 25)
                    {
                        foreach (var commentNode in dataNode.comments.data)
                        {
                            if (isGetByDate(commentNode.created_time) == true)
                             {
                            bool isExit = false;
                            for (int j = 0; j < countingArr.Count && isExit == false; j++)
                            {
                                //UserPost userPost in members
                                if (String.Compare(countingArr[j].id, commentNode.from.id) == 0)
                                {
                                    countingArr[j].comment = countingArr[j].comment + 1;
                                    isExit = true;
                                    // break;
                                }
                            }
                            if (isExit == false)
                            {
                                Counting tmp = new Counting(commentNode.from.id, 1, 0, 0, "", commentNode.from.name);
                                countingArr.Add(tmp);
                            }
                        }
                        }
                    }
                    else
                    {
                        var fbClient = new Facebook.FacebookClient();
                        fbClient.AccessToken = result.access_token;
                        dynamic resultTmp = dataNode.comments;
                        bool isContinute = true;
                        for (int i = 0; isContinute == true; i = i + 100)
                        {
                            string rq = "https://graph.facebook.com/" + dataNode.id + "/comments?limit=100&offset=" + i;
                            resultTmp = fbClient.Get(rq);
                            if (resultTmp.data != null)
                            {
                                foreach (var commentNode in resultTmp.data)
                                {
                                    if (isGetByDate(commentNode.created_time) == true)
                                    {
                                    bool isExit = false;
                                    for (int j = 0; j < countingArr.Count && isExit == false; j++)
                                    {
                                        //UserPost userPost in members
                                        if (String.Compare(countingArr[j].id, commentNode.from.id) == 0)
                                        {
                                            countingArr[j].comment = countingArr[j].comment + 1;
                                            isExit = true;
                                            // break;
                                        }
                                    }
                                    if (isExit == false)
                                    {
                                        Counting tmp = new Counting(commentNode.from.id, 1, 0, 0, "", commentNode.from.name);
                                        countingArr.Add(tmp);
                                    }
                                }
                                }
                                if (resultTmp.data.Count < 100)
                                    isContinute = false;
                            }
                            else
                                isContinute = false;
                        }

                        //do
                        //{
                        //    foreach (var commentNode in resultTmp.data)
                        //    {
                        //        bool isExit = false;
                        //        for (int i = 0; i < countingArr.Count && isExit == false; i++)
                        //        {
                        //            //UserPost userPost in members
                        //            if (String.Compare(countingArr[i].id, commentNode.from.id) == 0)
                        //            {
                        //                countingArr[i].comment = countingArr[i].comment + 1;
                        //                isExit = true;
                        //                // break;
                        //            }
                        //        }
                        //        if (isExit == false)
                        //        {
                        //            Counting tmp = new Counting(commentNode.from.id, 1, 0, 0);
                        //            countingArr.Add(tmp);
                        //        }
                        //    }

                        //    if (resultTmp.paging.next != null)
                        //        resultTmp = fbClient.Get(resultTmp.paging.next);
                        //    else
                        //        resultTmp = null;
                        //} while (resultTmp != null);
                    }
                }
            }
            // me = fbClient.Get(me.paging.next);
            //}

            return countingArr;
        }

        private List<Counting> getUserLikedList(dynamic result, ref List<Counting> countingArr)
        {
            if (countingArr == null)
                countingArr = new List<Counting>();

            // dynamic result1 = fbClient.Get("fql", new { q = "SELECT uid FROM user WHERE uid=me()" });

            // while (me.data != null)
            // {
            foreach (var dataNode in result.data)
            {
                if (isGetByDate(dataNode.created_time) == true)
                {
                //Post liked of user
                if (dataNode.likes != null)
                {
                    bool isExit = false;
                    int likeCount = 0;
                    if (dataNode.likes.Count < 25)
                        likeCount = dataNode.likes.Count;
                    else
                    {
                        var fbClient = new Facebook.FacebookClient();
                        fbClient.AccessToken = result.access_token;
                        dynamic resultTmp = dataNode.likes;
                        bool isContinute = true;
                        for (int i = 0; isContinute == true; i = i + 100)
                        {
                            string rq = "https://graph.facebook.com/" + dataNode.id + "/likes?limit=100&offset=" + i;
                            resultTmp = fbClient.Get(rq);
                            if (resultTmp.data != null)
                            {
                                likeCount += resultTmp.data.Count;
                                if (resultTmp.data.Count < 100)
                                    isContinute = false;
                            }
                            else
                                isContinute = false;
                        }
                    }

                    //bool isExit = false;
                    //int likeCount = dataNode.likes.data.Count;
                    //var fbClient = new Facebook.FacebookClient();
                    //fbClient.AccessToken = result.access_token;
                    //dynamic resultTmp = dataNode.likes;
                    //while (resultTmp.paging.next != null)
                    //{
                    //    fbClient = new Facebook.FacebookClient();
                    //    fbClient.AccessToken = result.access_token;
                    //    resultTmp = fbClient.Get(resultTmp.paging.next);
                    //    if (resultTmp.data != null)
                    //        likeCount += resultTmp.data.Count;
                    //}

                    for (int i = 0; i < countingArr.Count && isExit == false; i++)
                    {
                        //UserPost userPost in members
                        if (String.Compare(countingArr[i].id, dataNode.from.id) == 0)
                        {
                            countingArr[i].liked = countingArr[i].liked + likeCount;
                            isExit = true;
                            // break;
                        }
                    }
                    if (isExit == false)
                    {
                        Counting tmp = new Counting(dataNode.from.id, 0, likeCount, 0, "", dataNode.from.name);
                        countingArr.Add(tmp);
                    }
                }
            }

                //Comement liked of User
                if (dataNode.comments != null)
                {

                    dynamic resultTmp = dataNode.comments;

                    //do
                    //{
                    if (dataNode.comments.data.Count < 25)
                    {
                        foreach (var commentNode in resultTmp.data)
                        {
                            if (isGetByDate(commentNode.created_time) == true)
                            {
                            if (commentNode.like_count != 0)
                            {
                                int like_count = (int)commentNode.like_count;
                                bool isExit = false;

                                for (int i = 0; i < countingArr.Count && isExit == false; i++)
                                {
                                    //UserPost userPost in members
                                    if (String.Compare(countingArr[i].id, commentNode.from.id) == 0)
                                    {
                                        countingArr[i].liked = countingArr[i].liked + like_count;
                                        isExit = true;
                                        // break;
                                    }
                                }
                                if (isExit == false)
                                {
                                    Counting tmp = new Counting(commentNode.from.id, 0, like_count, 0, "", dataNode.from.name);
                                    countingArr.Add(tmp);
                                }
                            }
                        }
                        }

                    //    if (resultTmp.paging.next != null)
                    //        resultTmp = fbClient.Get(resultTmp.paging.next);
                    //    else
                    //        resultTmp = null;
                    //} while (resultTmp != null);
                }
                    //if (dataNode.comments.data.Count >= 25)
                else   {
                        bool isContinute = true;
                        for (int i = 0; isContinute == true; i = i + 100)
                        {
                            string rq = "https://graph.facebook.com/" + dataNode.id + "/comments?limit=100&offset=" + i;
                                                var fbClient = new Facebook.FacebookClient();
                    fbClient.AccessToken = result.access_token;
                            resultTmp = fbClient.Get(rq);
                            if (resultTmp.data != null)
                            {
                                if (resultTmp.data.Count < 100)
                                    isContinute = false;
                                foreach (var commentNode in resultTmp.data)
                                {
                                    if (isGetByDate(commentNode.created_time) == true)
                                    {
                                        if (commentNode.like_count != 0)
                                        {
                                            int like_count = (int)commentNode.like_count;
                                            bool isExit = false;

                                            for (int j = 0; j < countingArr.Count && isExit == false; j++)
                                            {
                                                //UserPost userPost in members
                                                if (String.Compare(countingArr[j].id, commentNode.from.id) == 0)
                                                {
                                                    countingArr[j].liked = countingArr[j].liked + like_count;
                                                    isExit = true;
                                                    // break;
                                                }
                                            }
                                            if (isExit == false)
                                            {
                                                Counting tmp = new Counting(commentNode.from.id, 0, like_count, 0, "", dataNode.from.name);
                                                countingArr.Add(tmp);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                isContinute = false;
                        }
                    }
                }
            }
            // me = fbClient.Get(me.paging.next);
            //}

            return countingArr;    
        }

        private List<Counting> getUserList(dynamic result)
        {
            List<Counting> countingArr = new List<Counting>();
            getUserCommentList(result, ref countingArr);
            getUserLikedList(result, ref countingArr);
            getUserPostList(result, ref countingArr);

           return countingArr;
        }

        private List<Counting> getPostCommentList(dynamic result, ref List<Counting> countingArr)
        {
            if (countingArr == null)
                countingArr = new List<Counting>();

            // dynamic result1 = fbClient.Get("fql", new { q = "SELECT uid FROM user WHERE uid=me()" });

            // while (me.data != null)
            // {
            foreach (var dataNode in result.data)
            {
                if (isGetByDate(dataNode.created_time) == true)
                {
                if (dataNode.comments != null)
                {
                    //int likeCount = 0;
                    //foreach()
                    bool isExit = false;
                    int commentCount = 0;// dataNode.comments.data.Count;
                    int count = dataNode.comments.data.Count;
                    if (count < 25)
                        commentCount = dataNode.comments.data.Count;
                    else
                    {
                        var fbClient = new Facebook.FacebookClient();
                        fbClient.AccessToken = accessToken;
                        dynamic resultTmp;// = dataNode.comments;
                        bool isContinute = true;
                        for (int i = 0; isContinute == true; i = i + 50)
                        {
                            string rq = "https://graph.facebook.com/" + dataNode.id + "/comments?limit=50&offset=" + i;
                            resultTmp = fbClient.Get(rq);
                            if (resultTmp.data != null)
                            {
                                commentCount += resultTmp.data.Count;
                                if (resultTmp.data.Count < 50)
                                    isContinute = false;
                            }
                            else
                                isContinute = false;

                        }
                    }

                    for (int i = 0; i < countingArr.Count && isExit == false; i++)
                    {
                        //UserPost userPost in members
                        if (String.Compare(countingArr[i].id, dataNode.id) == 0)
                        {
                            countingArr[i].comment = countingArr[i].comment + commentCount;
                            isExit = true;
                            // break;
                        }
                    }
                    if (isExit == false)
                    {
                        Counting tmp = new Counting(dataNode.id, commentCount, 0, 0, dataNode.created_time);
                        countingArr.Add(tmp);
                    }
                }
            }
            }
            // me = fbClient.Get(me.paging.next);
            //}
            //countingArr.Sort(Counting.commentCompare);
            return countingArr;

        }

        private List<Counting> getPostLikedList(dynamic result, ref List<Counting> countingArr)
        {
            if (countingArr == null)
                countingArr = new List<Counting>();

            // dynamic result1 = fbClient.Get("fql", new { q = "SELECT uid FROM user WHERE uid=me()" });

            // while (me.data != null)
            // {
            foreach (var dataNode in result.data)
            {
                if (isGetByDate(dataNode.created_time) == true){
                //Post liked of user
                if (dataNode.likes != null)
                {
                    //int likeCount = 0;
                    //foreach()
                    bool isExit = false;
                    int likeCount = 0;
                    if (dataNode.likes.data.Count < 25)
                        likeCount = dataNode.likes.data.Count;
                    else
                    {
                        var fbClient = new Facebook.FacebookClient();
                        fbClient.AccessToken = result.access_token;
                        dynamic resultTmp = dataNode.likes;
                        bool isContinute = true;
                        for (int i = 0; isContinute == true; i = i + 50)
                        {
                            string rq = "https://graph.facebook.com/" + dataNode.id + "/likes?limit=50&offset=" + i;
                            resultTmp = fbClient.Get(rq);
                            if (resultTmp.data != null)
                            {
                                likeCount += resultTmp.data.Count;
                                if (resultTmp.data.Count < 50)
                                    isContinute = false;
                            }
                            else
                                isContinute = false;
                        }
                    }

                    for (int i = 0; i < countingArr.Count && isExit == false; i++)
                    {
                        //UserPost userPost in members
                        if (String.Compare(countingArr[i].id, dataNode.id) == 0)
                        {
                            countingArr[i].liked = countingArr[i].liked + likeCount;
                            isExit = true;
                            // break;
                        }
                    }
                    if (isExit == false)
                    {
                        Counting tmp = new Counting(dataNode.id, 0, likeCount, 0, dataNode.created_time);
                        countingArr.Add(tmp);
                    }
                }
            }
            }
            // me = fbClient.Get(me.paging.next);
            //}

            return countingArr;
        }

        private List<Counting> getPostList(dynamic result)
        {
            List<Counting> countingArr = new List<Counting>();
            getPostLikedList(result, ref countingArr);
            getPostCommentList(result, ref countingArr);      

            return countingArr;
        }

        private bool isGetByDate(string date)
        {
            if (String.Compare(startDate, "") == 0 || String.Compare(endDate, "") == 0)
                return true;
            DateTime myDate = Post.convertDateTime(date);
            DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.CurrentCulture);
            DateTime end = DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.CurrentCulture);

            if (DateTime.Compare(myDate, start) >= 0)
                if ( DateTime.Compare(end, myDate) >= 0)
                return true;
            return false;
        }

    }
}

