$(document).ready(function () {

    var allMessages = [];

    var chat = $.connection.socialNetworkHub;

    chat.client.getMessage = function (user, message, users) {

        var usersInPage = getDialogUsers();

        for (var i = 0; i < usersInPage.length; i++) {

            if (users.indexOf(usersInPage[i]) == -1) {
                var mainDiv = $('<div id="modal-message" class="col-md-4 row" style="height: 100px; overflow: hidden; max-width: 34%; ' +
                        'position: fixed; top: 80%; left: 0%; background-color: rgba(128, 128, 128, 0.3)">');

                var photoDiv = $('<div class="col-md-3">');
                photoDiv.append('<img style="height: 90px;" class="img-responsive" src="data:image/*;base64,' + user.Photo + '" />');

                var descriptionDiv = $('<div class="col-md-9">');
                descriptionDiv.append('<p>' + user.Name + '</p><br/>');
                descriptionDiv.append('<p>' + message + '</p><br/>');

                mainDiv.append(photoDiv);
                mainDiv.append(descriptionDiv);

                $("body").append(mainDiv);

                setTimeout(function () {
                    $("#modal-message").remove();
                }, 5000);

                return;
            }
        }

        loadDialog();
    };

    $.connection.hub.start().done(function () {
        $("#send").click(function () {

            var users = getDialogUsers();

            var message = {
                users: users,
                message: $("#text-message").val()
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

                    chat.server.send(users, $("#text-message").val(), $("#user-id").val());

                    loadDialog();
                },
                error: function () {
                    alert("Ошибка отправки сообщения");
                }
            });
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
        var users = [];
        var inputs = $(".users");
        for(var i = 0; i < inputs.length; i++)
        {
            users.push(+$(inputs[i]).val());
        }

        return users;
    }
});