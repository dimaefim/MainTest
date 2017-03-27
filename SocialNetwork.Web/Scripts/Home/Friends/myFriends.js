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

        loadUsers();
    });

    $("#my-requests").click(function () {

        tab = 3;

        $("#all-friends").removeClass();
        $("#requests").removeClass();
        $("#my-requests").removeClass();

        $("#my-requests").addClass("active");

        loadUsers();
    });

    $("#nameFilter").keyup(function () {
        var result = [];
        result.length = 0;
        var filter = $(this).val();

        if (filter.length == 0) {
            showUsers(allUsers);

            return;
        }

        for (var i = 0; i < allUsers.length; i++) {
            var fullName1 = allUsers[i].Surname + " " + allUsers[i].Name;
            var fullName2 = allUsers[i].Name + " " + allUsers[i].Surname;

            if (fullName1.indexOf(filter) != -1 || fullName2.indexOf(filter) != -1) {
                result.push(allUsers[i]);
            }
        }

        showUsers(result);
    });

    loadUsers();

    function loadUsers() {

        var url = "/Home/";

        if (tab == 1) {
            url += "GetMyFriends";
        } else if (tab == 2) {
            url += "GetRequests";
        } else if (tab == 3) {
            url += "GetMyRequests";
        }

        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allUsers.length = 0;
                allUsers = data;

                showUsers(allUsers);
            },
            error: function () {
                alert("Ошибка получения пользователей");
            }
        });
    }

    function showUsers(users) {
        var results = $('#my-friends');
        results.empty();

        if (users.length == 0) {
            results.append('<p>Поиск не дал результатов</p>');

            return;
        }

        var status = "";

        if (tab == 1) {
            status += "Убрать из друзей";
        } else if (tab == 2) {
            status += "Хочет дружить. Добавить в друзья";
        } else if (tab == 3) {
            status += "Отменить запрос дружбы";
        }

        for (var i = 0; i < users.length; i++) {

            var mainDiv = $('<div class="col-md-12 row">');

            var photoDiv = $('<div class="col-md-3">');
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + btoa(users[i].MainPhoto) + '" />');

            var descriptionDiv = $('<div class="col-md-9">');
            descriptionDiv.append('<input id="id" type="hidden" value="' + users[i].Id + '"/>');
            descriptionDiv.append('<a href="/Home/ShowUserPage/' + users[i].Id + '">' + users[i].Surname + " " + users[i].Name + '</a><br/>');
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