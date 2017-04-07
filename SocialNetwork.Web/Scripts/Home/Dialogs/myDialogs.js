$(document).ready(function () {

    var allDialogs = [],
        chat = $.connection.socialNetworkHub,
        filterByName = $("#nameFilter");

    chat.client.getMessage = function (user, message, users) {

        var mainDiv = $('<div id="modal-message" class="col-md-4 row" style="height: 100px; overflow: hidden; max-width: 34%; ' +
                        'position: fixed; top: 80%; left: 0%; background-color: rgba(0, 0, 0, 1.0)">');

        var photoDiv = $('<div class="col-md-3">');
        photoDiv.append('<img style="height: 90px;" class="img-responsive" src="data:image/*;base64,' + user.Photo + '" />');

        var descriptionDiv = $('<div class="col-md-9">');
        descriptionDiv.append('<p style="color: white;">' + user.Name + '</p><br/>');
        descriptionDiv.append('<p style="color: white;">' + message + '</p><br/>');

        mainDiv.append(photoDiv);
        mainDiv.append(descriptionDiv);

        $("body").append(mainDiv);

        setTimeout(function () {
            $("#modal-message").remove();
        }, 5000);

        loadDialogs();
    }

    filterByName.keyup(function () {
        filterDialogs(allDialogs);
    });

    loadDialogs();

    function loadDialogs() {
        $.ajax({
            type: "POST",
            url: "/Dialogs/GetAllDialogs",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allDialogs.length = 0;
                allDialogs = data;
                filterDialogs(allDialogs);
            },
            error: function() {
                alert("Ошибка получения диалогов");
            }
        });
    }

    function showDialogs(dialogs) {
        var results = $('#all-dialogs');
        results.empty();

        if (dialogs.length == 0) {
            results.append('<p>Поиск не дал результатов</p>');

            return;
        }

        for (var i = 0; i < dialogs.length; i++) {

            var mainDiv = $('<div class="col-md-12 row dialog">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + dialogs[i].Photo + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + dialogs[i].Id + '"/>');
            descriptionDiv.append('<p>' + dialogs[i].Name + '</p><br/>');
            descriptionDiv.append('<p>' + dialogs[i].LastMessage + '</p><br/>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            results.append(mainDiv);
            results.append('<hr/>');
        }

        results.find(".dialog").click(function () {
            window.location.href = "/Dialogs/OpenDialogByDialogId/" + $(this).find("#id").val();
        });
    }

    function filterDialogs(dialogs) {
        dialogs = filterUsersByName(dialogs);
        showDialogs(dialogs);
    }

    function filterUsersByName(dialogs) {
        var resultAfterNameFilter = [],
            filterName = filterByName.val().toLowerCase();

        if (filterName.length == 0) {

            return dialogs;
        }

        for (var i = 0; i < dialogs.length; i++) {
            var fullName = (dialogs[i].Name).toLowerCase();

            if (fullName.indexOf(filterName) != -1) {
                resultAfterNameFilter.push(dialogs[i]);
            }
        }

        return resultAfterNameFilter;
    }
});