$(document).ready(function () {

    var allMessages = [];

    $("#send").click(function () {
        $.ajax({
            type: "POST",
            url: "/Home/SendMessage?id=" + $("#id").val() + "&message=" + $("#text-message").val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.response == "false") {
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
        $.ajax({
            type: "POST",
            url: "/Home/GetMessages/" + $("#id").val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
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

        if (messages.length == 0) {
            result.append('<p class="text-center">История переписки пуста</p>');

            return;
        }

        for (var i = 0; i < messages.length; i++) {

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + messages[i].Photo + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + messages[i].Id + '"/>');
            descriptionDiv.append('<p>' + messages[i].Author + ' - ' + messages[i].TimeOfSend + '</p><br/>');
            descriptionDiv.append('<p>' + messages[i].Text + '</p><br/>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            result.append(mainDiv);
            result.append('<hr/>');
        }

        var block = document.getElementById("messages");
        block.scrollTop = block.scrollHeight;
    }
});