$(document).ready(function () {

    var allDialogs = [];

    $("#nameFilter").keyup(function () {
        filterDialogs();
    });

    loadDialogs();

    function loadDialogs() {
        $.ajax({
            type: "POST",
            url: "/Home/GetAllDialogs",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allDialogs.length = 0;
                allDialogs = data;
                showDialogs(allDialogs);
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
            window.location.href = "/Home/OpenDialogByDialogId/" + $(this).find("#id").val();
        });
    }

    function filterDialogs() {
        filterUsersByName();
    }

    function filterUsersByName() {
        
    }
});