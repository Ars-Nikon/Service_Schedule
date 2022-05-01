$("#Update").click(function () {
    UpdateSpec();
});

$(document).on("dblclick", ('tr', '.user'), function () {
    let id = $(this).attr('id');
    $("#div_0").load(`/admin/SpecInfoSegment?id=${id}`);
});

async function AJAXSubmit(oFormElement) {
    var resultElement = oFormElement.elements.namedItem("result");
    const formData = new FormData(oFormElement);

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            window.location.href = '/';
        }

        resultElement.value = 'Result: ' + response.status + ' ' +
            response.statusText;
    } catch (error) {
        console.error('Error:', error);
    }
}


function send_form() {
    let forminfo = $("#FormInfo");
    forminfo.empty();
    let form = $('#SpecEdit');
    let msg = form.serialize();

    $.ajax({
        type: 'POST',
        url: '../admin/EditSpec',
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


function DeleteSpec() {
    let form = $('#SpecEdit');
    let msg = form.serialize();
    $.ajax({
        type: 'POST',
        url: '../admin/DeleteUser',
        data: msg,
        contentType: false,
        processData: false,
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
    $("#div_1").load("/admin/SpecsList");
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