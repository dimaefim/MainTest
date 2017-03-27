$(document).ready(function () {

    var allUsers = [];
    var tab = 1;

    $("#all-friends").click(function () {

        tab = 1;

        $("#all-friends").removeClass();
        $("#requests").removeClass();
        $("#my-requests").removeClass();

        $("#all-friends").addClass("active");

        loadUsers();
    });

    $("#requests").click(function () {

        tab = 2;

        $("#all-friends").removeClass();
        $("#requests").removeClass();
        $("#my-requests").removeClass();

        $("#requests").addClass("active");

        $.ajax({
            type: "POST",
            url: "/Home/GetAllUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allUsers.length = 0;
                allUsers = data;
                showRequests(allUsers);
            },
            error: function () {
                alert("Ошибка получения пользователей");
            }
        });
    });

    $("#my-requests").click(function () {

        tab = 3;

        $("#all-friends").removeClass();
        $("#requests").removeClass();
        $("#my-requests").removeClass();

        $("#my-requests").addClass("active");

        $.ajax({
            type: "POST",
            url: "/Home/GetAllUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allUsers.length = 0;
                allUsers = data;
                showMyRequests(allUsers);
            },
            error: function () {
                alert("Ошибка получения пользователей");
            }
        });
    });

    loadUsers();

    function loadUsers() {
        $.ajax({
            type: "POST",
            url: "/Home/GetAllUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allUsers.length = 0;
                allUsers = data;

                if (tab == 1) {
                    showMyFriends(allUsers);
                } else if (tab == 2) {
                    showRequests(allUsers);
                } else if (tab == 3) {
                    showMyRequests(allUsers);
                }
            },
            error: function () {
                alert("Ошибка получения пользователей");
            }
        });
    }

    function showMyFriends(users) {
        var results = $('#my-friends');
        results.empty();

        for (var i = 0; i < users.length; i++) {
            var status = "";

            if (users[i].Status != 2) {
                continue;
            }

            status = "Убрать из друзей";

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + btoa(users[i].MainPhoto) + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + users[i].Id + '"/>');
            descriptionDiv.append('<a href="#">' + users[i].Surname + " " + users[i].Name + '</a><br/>');
            descriptionDiv.append('<button type="button" class="btn btn-default">' + status + '</button>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            results.append(mainDiv);
            results.append('<hr/>');
        }

        results.find(".btn").click(function () {
            var parrent = $(this).parent();
            addRequestInFriendsList(parrent.find("#id").val());
        });
    }

    function showRequests(users) {
        var results = $('#my-friends');
        results.empty();

        for (var i = 0; i < users.length; i++) {
            var status = "";

            if (users[i].Status != 4) {
                continue;
            }

            status = "Хочет дружить. Добавить в друзья";

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + btoa(users[i].MainPhoto) + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + users[i].Id + '"/>');
            descriptionDiv.append('<a href="#">' + users[i].Surname + " " + users[i].Name + '</a><br/>');
            descriptionDiv.append('<button type="button" class="btn btn-default">' + status + '</button>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            results.append(mainDiv);
            results.append('<hr/>');
        }

        results.find(".btn").click(function () {
            var parrent = $(this).parent();
            addRequestInFriendsList(parrent.find("#id").val());
        });
    }

    function showMyRequests(users) {
        var results = $('#my-friends');
        results.empty();

        for (var i = 0; i < users.length; i++) {
            var status = "";

            if (users[i].Status != 3) {
                continue;
            }

            status = "Отменить запрос дружбы";

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + btoa(users[i].MainPhoto) + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + users[i].Id + '"/>');
            descriptionDiv.append('<a href="#">' + users[i].Surname + " " + users[i].Name + '</a><br/>');
            descriptionDiv.append('<button type="button" class="btn btn-default">' + status + '</button>');

            mainDiv.append(photoDiv);
            mainDiv.append(descriptionDiv);

            results.append(mainDiv);
            results.append('<hr/>');
        }

        results.find(".btn").click(function () {
            var parrent = $(this).parent();
            addRequestInFriendsList(parrent.find("#id").val());
        });
    }

    function addRequestInFriendsList(id) {
        $.ajax({
            type: "POST",
            url: "/Home/AddRequestToFriendList/" + id,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                switch (data.response) {
                    case "me":
                        alert("Невозможно добавить в друзья самого себя");
                        break;
                    case "no friend":
                        break;
                    case "i accept":
                        break;
                    case "request":
                        break;
                    default:
                        alert("Ошибка операции");
                        break;
                }

                loadUsers();
            },
            error: function () {
                alert("Ошибка операции");
            }
        });
    }


});