
//var script = document.createElement("script");
//script.type = "text/javascript";
//script.src = "~/lib/jquery/dist/jquery.js";
//document.getElementsByTagName('head')[0].appendChild(script); 

//$(function () {
//    alert("123");
//});

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/signalrHubs").build();

connection.on("GetMsg", function (user, message,timeStr) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "[" + timeStr + "]" + user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    $("#messagesList").append(li);
});

//进入聊天室时，拉取历史聊天记录
connection.on("GetHistoryMsg", function (history) {
    $("#messagesList").html("");
    var historyAttr = JSON.parse(history);
    if (historyAttr.length > 0) {
        $.each(historyAttr, function (index,data) {
            var li = document.createElement("li");
            li.textContent = "[" + data.BizCreateTime + "]：" + data.UserName + " 说 " + data.Msg;
            $("#messagesList").append(li);
        });
    }
});
connection.on("print", function (message) {
    console.log(message);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

$("#sendButton").on("click", function (event) {
    var user = $("#userInput").val();
    var message = $("#messageInput").val();
    //调用到hubs中的SendMsg方法
    connection.invoke("SendMsg", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});