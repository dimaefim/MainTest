$(document).ready(function () {

    loadUsers();

    function loadUsers() {
        $.ajax({
            type: "POST",
            url: "/Home/GetAllUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var results = $('#all-users');
                results.empty();

                for (var i = 0; i < data.length; i++) {
                    results.append('<div class="col-md-12 row">' +
                        '<div class="col-md-3"> <img class="user-photo img-responsive" src="data:image/*;base64,'
                        + atob(data[i].MainPhoto) + '" /> </div>' + '<div class="col-md-9"> <input type="hidden" value="' + data[i].Id + '"/>' + 
                        '<a href="#">' + data[i].Surname + " " + data[i].Name + '</a>' + '<br/><button type="button" class="btn btn-default">Дружить</button>' + 
                        '</div></div> <hr/>');
                }
            },
            error: function() {
                alert("Ошибка получения пользователей!");
            }
    });
    }
});