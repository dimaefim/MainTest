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
        filterUsers();
    });

    $("#startAge").change(function () {
        addValuesToSelects(1);
        filterUsers();
    });

    $("#endAge").change(function () {
        addValuesToSelects(2);
        filterUsers();
    });

    addValuesToSelects(0);
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
            photoDiv.append('<img class="user-photo img-responsive" src="data:image/*;base64,' + users[i].MainPhotoString + '" />');

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

    function addValuesToSelects(val) {
        var startAge = $("#startAge").val();
        var endAge = $("#endAge").val();

        var startSelect = $("#startAge");
        var endSelect = $("#endAge");

        if (val == 0) {

            endSelect.empty();
            startSelect.empty();

            for (var i = 10; i <= 80; i++) {
                startSelect.append('<option id="start' + i + '">' + i + '</option>');
                endSelect.append('<option id="end' + i + '">' + i + '</option>');
            }

            $("#endAge").children().last().attr("selected", "selected");
        }

        if (val == 1) {

            endSelect.empty();

            for (var i = +startAge; i <= 80; i++) {
                endSelect.append('<option id="end' + i + '">' + i + '</option>');
            }

            if (+endAge < +startAge) {
                endAge = +startAge;
            }

            $("#end" + endAge).attr("selected", "selected");
        }

        if (val == 2) {

            startSelect.empty();

            for (var i = 10; i <= +endAge; i++) {
                startSelect.append('<option id="start' + i + '">' + i + '</option>');
            }

            if (+startAge > +endAge) {
                startAge = +endAge;
            }

            $("#start" + startAge).attr("selected", "selected");
        }
    }

    function filterUsers() {
        var result = [];
        result.length = 0;

        result = filterUsersByName(allUsers);
        result = filterUsersByAge(result);

        showUsers(result);
    }

    function filterUsersByName(users) {
        var resultAfterNameFilter = [];
        resultAfterNameFilter.length = 0;
        var filterName = $("#nameFilter").val().toLowerCase();

        if (filterName.length == 0) {

            return users;
        }

        for (var i = 0; i < users.length; i++) {
            var fullName = (users[i].Surname + " " + users[i].Name).toLowerCase();
            var fullNameReverse = (users[i].Name + " " + users[i].Surname).toLowerCase();

            if (fullName.indexOf(filterName) != -1 || fullNameReverse.indexOf(filterName) != -1) {
                resultAfterNameFilter.push(users[i]);
            }
        }

        return resultAfterNameFilter;
    }

    function filterUsersByAge(users) {
        var resultAfterAgeFilter = [];
        resultAfterAgeFilter.length = 0;

        var startAge = +$("#startAge").val();
        var endAge = +$("#endAge").val();

        for (var i = 0; i < users.length; i++) {
            var dateOfBirth = new Date(users[i].DateOfBirthString.substr(6, 4),
                                       users[i].DateOfBirthString.substr(3, 2),
                                       users[i].DateOfBirthString.substr(0, 2));
            var nowDate = new Date();

            var difference = (nowDate - dateOfBirth) / 31536000000;

            if (difference >= startAge && difference <= endAge) {
                resultAfterAgeFilter.push(users[i]);
            }
        }

        return resultAfterAgeFilter;
    }
});