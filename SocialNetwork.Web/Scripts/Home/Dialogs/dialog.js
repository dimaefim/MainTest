$(document).ready(function () {

    var allMessages = [];

    $("#send").click(function () {
        var users = getDialogUsers();

        var message = {
            users: users,
            message : $("#text-message").val()
        }

        $.ajax({
            type: "POST",
            url: "/Dialogs/SendMessage/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(message),
            success: function (data) {
                if (data.response == false) {
                    alert("Ошибка отправки сообщения");
                    return;
                }

                $("#text-message").val('');

                loadDialog();
            },
            error: function () {
                alert("Ошибка отправки сообщения");
            }
        });
    });

    loadDialog();

    function loadDialog() {

        var users = getDialogUsers();

        $.ajax({
            type: "POST",
            url: "/Dialogs/GetMessages/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(users),
            success: function (data) {
                allMessages.length = 0;
                allMessages = data;
                showMessages(allMessages);
            },
            error: function () {
                alert("Ошибка получения сообщений");
            }
        });
    }

    function showMessages(messages) {
        var result = $('#messages');
        result.empty();

        if (messages.messages.length == 0) {
            result.append('<p class="text-center">История переписки пуста</p>');

            return;
        }

        for (var i = 0; i < messages.messages.length; i++) {

            var photo = "";
            var fullName = "";

            for (var j = 0; j < messages.userData.length; j++) {

                if (messages.messages[i].UserId == messages.userData[j].UserId) {
                    photo = messages.userData[j].Photo;
                    fullName = messages.userData[j].Author;
                    break;
                }

            }

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + photo + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + messages.messages[i].Id + '"/>');
            descriptionDiv.append('<p>' + fullName + ' - ' + messages.messages[i].TimeOfSend + '</p><br/>');
            descriptionDiv.append('<p>' + messages.messages[i].Text + '</p><br/>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            result.append(mainDiv);
            result.append('<hr/>');
        }

        var block = document.getElementById("messages");
        block.scrollTop = block.scrollHeight;
    }

    function getDialogUsers() {
        var inputs = [];
        var users = [];
        var inputs = $(".users");
        for(var i = 0; i < inputs.length; i++)
        {
            users.push(+$(inputs[i]).val());
        }

        return users;
    }
});