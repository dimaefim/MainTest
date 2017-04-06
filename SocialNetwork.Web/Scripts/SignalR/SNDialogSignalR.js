$(document).ready(function () {

    var chat = $.connection.socialNetworkHub;

    $.connection.hub.start().done(function () {
        chat.server.connect($("#user-id").val());
    });
});