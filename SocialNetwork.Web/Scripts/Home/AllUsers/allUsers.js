﻿$(document).ready(function () {

    var allUsers = [];

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
        $.ajax({
            type: "POST",
            url: "/Home/GetAllUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                allUsers.length = 0;
                allUsers = data;
                showUsers(allUsers);
            },
            error: function() {
                alert("Ошибка получения пользователей");
            }
        });
    }

    function showUsers(users) {
        var results = $('#all-users');
        results.empty();

        if (users.length == 0) {
            results.append('<p>Поиск не дал результатов</p>');

            return;
        }

        for (var i = 0; i < users.length; i++) {
            var status = "";

            switch(users[i].Status) {
                case 3:
                    status = "Отменить запрос дружбы";
                    break;
                case 4:
                    status = "Хочет дружить. Добавить в друзья";
                    break;
                case 5:
                    status = "Дружить";
                    break;
            }

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
                        alert("Теперь вы друзья!");
                        break;
                    case "request":
                        alert("Запрос успешно отправлен");
                        break;
                    default:
                        alert("Ошибка отправки запроса");
                        break;
                }

                loadUsers();
            },
            error: function () {
                alert("Ошибка создания запроса на добавление в друзья");
            }
        });
    }


});