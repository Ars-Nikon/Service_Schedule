$("#UpdateUsers").click(function () {
    $("#div_1").load("/admin/UsersList");
});



$(document).on("dblclick", ('tr', '.user'), function () {
    let id = $(this).attr('id');
    $("#div_0").load(`/admin/UserInfoSegment?id=${id}`);
});



function send_form() {
    let forminfo = $("#FormInfo");
    forminfo.empty();
    let form = $('#AccountEdit');
    let msg = form.serialize();
    $.ajax({
        type: 'POST',
        url: '../admin/EditUser',
        data: msg,
        success: function (data) {
            forminfo.removeClass();
            if (data == null) {
                forminfo.empty();
            }
            else {
                if (data.success) {
                    forminfo.append(data.resultMessage);
                    forminfo.addClass("text-info");
                }
                else {
                    forminfo.append(data.errors);
                    forminfo.addClass("text-danger");
                }
                UpdateSpec();
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });
}


function DeleteUser() {
    let form = $('#AccountEdit');
    let msg = form.serialize();
    $.ajax({
        type: 'POST',
        url: '../admin/DeleteUser',
        data: msg,
        success: function (data) {
            $("#div_0").empty();
            UpdateSpec();
        },
        error: function () {
            alert('Ошибка!');
        }
    });
}

function UpdateSpec() {
    $("#div_1").load("/admin/UsersList");
}


function show_hide_password(target) {
    var input = document.getElementById('password-input1');
    if (input.getAttribute('type') == 'password') {
        target.classList.add('view');
        input.setAttribute('type', 'text');
    } else {
        target.classList.remove('view');
        input.setAttribute('type', 'password');
    }
    return false;
}