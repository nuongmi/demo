(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=430464820399215";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));




var loaded = false;
var ind = 10;
$(document).ready(function () {

    $("#span").click(function () {
        if (startCalendar == null || endCalendar == null) {
            var dateTime = '<span class="label">From</span> @Html.TextBox("startCalendar");' + '<span class="label">Till</span> @Html.TextBox("endCalendar")';
            $("#dateTime").append(dateTime);
            createCalendar();
        }
    });

    var buttonLoadingText = "<img src='images/loader.gif' alt='' /> Loading..";
    //$.ajaxSetup({ cache: false }); // disabling cache, omit if u dont need
    $(document).scroll(function () {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - 50) {
            if (!loaded) {
                loaded = true;
                loadMore();
                ind = ind + 10;
                loaded = false;
            }
        }
    });

    function loadMore() {
        $("#loadButton").html(buttonLoadingText);
        //$("#content").append("<div><a href='#'>here is link</a></div>");
            
        $.ajax({
            url: 'Home/GetNextPost',//'@Url.Action("GetNextPost")',
            type: 'Get',
            dataType: "html",
            data: { start: ind},
            success: function (result) {
                result = JSON.parse(result);

                for (var i = 0; i < result.length; i++) {
                    var ct = '<div class="prod_box">' +
                   '<a href="' + result[i].from.link + '"><img src="' + result[i].from.pictureUrl + '" alt="" title="" class="prod_img"/></a>' +
                       '<div class="prod_details"  style="font-size:13px">'+
                           '<div>'+
                               '<a href="' + result[i].from.link + '" class="name_link">' + result[i].from.name + '</a>'+
                               '<p>' + result[i].message + '</p>'+
                           '</div>'+
                           '<div class="count_LC">' +
                               ' <img src="Images/like_icon.png" />' +
                               'Like: ' + result[i].liked + " <a  style='color:#3b5998;text-decoration:none;' " + 'onclick="showComment(' + "'" + result[i].id + "'" + ')">' + "Comment: " + result[i].comment + '</a>' + '  <a  style="color:gray">' + result[i].dateTime + '</a>' +
                           '</div>'+
                           '<div id ="' + result[i].id + '">'+

                          '</div>'+
                       '</div>'+               
                 '</div> ';
                     
                    $("#post").append(ct);
                }
            }
        });
    }
});

function showComment(postID) {
    //alert("*****");
    var id = "#" + postID;
    var value = document.getElementById(postID).innerHTML;
    if (value == "") {
        var p = postID;
        console.log(id);
        //$(id).append(postID);
        $.ajax({
            url: 'Home/GetComment',//'@Url.Action("GetComment")',
            type: 'Get',
            dataType: "html",
            data: { postID: p },
            success: function (result) {

                result = JSON.parse(result);
                console.log(result);

                for (var i = 0; i < result.length; i++) {
                    var ct = '<div class="prod_box_child">' +
                   '<a href="' + result[i].from.link + '"><img src="' + result[i].from.pictureUrl + '" alt="" title="" class="prod_img_child"/></a>' +
                       '<div class="prod_details_child">' +
                           '<div>' +
                               '<a href="' + result[i].from.link + '" class="name_link">' + result[i].from.name + '</a>' +
                               '<p>' + result[i].message + '</p>' +
                           '</div>' +
                          '</div>' +
                       '</div>' +
                 '</div> ';

                    $(id).append(ct);
                }
            }
        });
    }
    else {
        $(id).toggle();
    }
}


var startCalendar = null;
var endCalendar = null;
function createCalendar() {
    var value = document.getElementById("time").innerHTML;
    //alert(value);
    //if (value == "Choose time")
    //    document.getElementById("time").innerHTML = "All time";
    //else
    //    document.getElementById("time").innerHTML = "Choose time";
    if (startCalendar == null || endCalendar == null) {
        //var dateTime = '<span class="label">From</span><input type="text" id="startCalendar" /> <span class="label">Till</span> <input typt="text" id = "endCalendar" />';
        //$("#dateTime").append(dateTime);
        startCalendar = new dhtmlXCalendarObject("startCalendar");
        startCalendar.setDateFormat("%d/%m/%Y %H:%i");

        endCalendar = new dhtmlXCalendarObject("endCalendar");
        endCalendar.setDateFormat("%d/%m/%Y %H:%i");
    }
    else {
        $('startCalendar').show;
        $('endCalendar').show;
    }
}


function hideCalendar() {
    
    if (startCalendar != null && endCalendar != null) {
        $('startCalendar').hide;
        $('endCalendar').hide;
    }
} 

function testDateTime() {
    var date1String = document.getElementById('startCalendar').value;
    var date2String = document.getElementById('endCalendar').value;
    var date1 = new Date(date1String);
    var date2 = new Date(date2String);
    if (date1 > date2) {
        alert("Khoảng thời gian khong hợp lệ");
        return false;
    }
    else {
        //alert("Hop le ddd" + date1String + "  " + date2String);
        return true;
    }
}