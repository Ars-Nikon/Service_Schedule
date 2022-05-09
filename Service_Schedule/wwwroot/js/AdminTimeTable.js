function UpdateCalendar() {
    let specSelect = $("#Specs");
    let mounthSelect = $("#Months");
    let Id = specSelect.val();
    let Months = mounthSelect.val();
    $("#div_0").load(`/admin/CalendarSegment?Id=${Id}&Mounth=${Months}`);
}

function ChangeActive(element) {
    let lastActiveEl = $(".bg-info");
    if (lastActiveEl != undefined) {
        lastActiveEl.removeClass("bg-info");
    }
    element.addClass("bg-info");
}

$(document).on("click", ('td', '.calendar'), function () {
    let msg = {
        IdDay: $(this).text(),
        IdMounth: $("#Months").val(),
        IdSpec: $("#Specs").val()
    }

    if (msg.IdDay == ' ')
        return;

    ChangeActive($(this));


    PostRequestForInfoPage(msg);
});

function PostRequestForInfoPage(msg) {
    $.ajax({
        type: 'POST',
        url: '../admin/TimeTableDay',
        data: msg,
        success: function (data) {
            {
                UpdateInfo(data)
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });
}

function UpdateInfo(data) {
    let DivInfo = $("#div_1");
    DivInfo.empty();
    DivInfo.append(data);
}


function AddNewTimeTableTime() {
    let msg = {
        Status: $("#AddNewStatus").val(),
        Start: $("#AddNewStart").val(),
        Finish: $("#AddNewFinish").val(),
        Id_TimeTableDate: $("#GuidTimeTable").val()
    }
    if (msg.Start == "") {
        alert("Заполните время начало приема");
        return;
    }
    else if (msg.Finish == "") {
        alert("Заполните время окончания приема");
        return;
    }
    else if (msg.Start >= msg.Finish) {
        alert("Начало приема не может быть позже (равно) окончанию");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '../admin/AddNewTimeTableTime',
        data: msg,
        success: function (data) {
            if (data.status != "Ok") {
                alert(data.message);
            }
            else {
                PostRequestForInfoPage({ GuidTimeTable: $("#GuidTimeTable").val() });
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });
}


function CreateTimeTableDate() {
    var date = $("#TimeTableDate").val();
    let msg = {
        date: date,
        IdSpec: $("#Specs").val()
    }


    $.ajax({
        type: 'POST',
        url: '../admin/CreateTimeTableDate',
        data: msg,
        success: function (data) {
            {
                PostRequestForInfoPage({ GuidTimeTable: data });
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });
}


function DeleteTimeTableTime(Id) {
    let msg = {
        Id_TimeTableTime: Id,
    }
    let shure = confirm("Точно УДАЛИТЬ это расписание?");
    if (shure) {
        $.ajax({
            type: 'POST',
            url: '../admin/DeleteTimeTableTime',
            data: msg,
            success: function (data) {
                {
                    if (data.status != "Ok") {
                        alert(data.message);
                    }
                    else {
                        PostRequestForInfoPage({ GuidTimeTable: $("#GuidTimeTable").val() });
                    }
                }
            },
            error: function () {
                alert('Ошибка!');
            }
        });
    }
}

function UpdateTimeTableTime(Id) {

    let status = $("tr#" + Id).children("td#Status").children("select").val();
    let discription = $("tr#" + Id).children("td#Discription").children("textarea").val();

    let msg = {
        Id_TimeTableTime: Id,
        Status: status,
        Discription: discription
    }

    $.ajax({
        type: 'POST',
        url: '../admin/UpdateTimeTableTime',
        data: msg,
        success: function (data) {
            {
                if (data.status != "Ok") {
                    alert(data.message);
                }
                PostRequestForInfoPage({ GuidTimeTable: $("#GuidTimeTable").val() });
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });

}


function UserModal(Id) {
    $('#myModal').modal();
    $("#modalBody").load(`/account/UserInfoRecord?Id=${Id}`);
}