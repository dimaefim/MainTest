$(document).ready(function () {

    var chat = $.connection.socialNetworkHub;

    chat.client.getMessage = function (user, message, users) {
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
    };

    $.connection.hub.start().done(function () {
        chat.server.connect($("#user-id").val());
    });
});