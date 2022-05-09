function RecordModal(Id) {
    $('#myModal').modal();
    let data = $('#Date').val();
    let start = $("tr#" + Id).children("td#Start").text();
    let end = $("tr#" + Id).children("td#End").text();
    $('#TimeVisit').text(`${start} - ${end}`)
    $('#ModalDate').text(data)
    $("#HidenGuidtime").val(Id);
}

function Recording() {

    let Id = $("#HidenGuidtime").val();
    $.ajax({
        type: 'POST',
        url: '/Home/Recording',
        data: { Id: Id },
        success: function (data) {
            {
                if (data.status != "Ok") {
                    $('#myModal').modal('hide');
                    alert(data.message);
                    let GuidTimeTable = { GuidTimeTable: $("#GuidTimeTable").val() };
                    PostRequestForInfoPage(GuidTimeTable);
                }
                else {
                    window.location.replace("../MyRecord");
                }
            }
        },
        error: function () {
            alert('Ошибка!');
        }
    });

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
        GuidTimeTable: $(this).attr('id')
    }
    if ($(this).text() == " ")
        return;
    ChangeActive($(this));
    if ($(this).attr('id') == undefined) {
        EmptyInfo();
        return;
    }

    PostRequestForInfoPage(msg);
});


function EmptyInfo() {
    let DivInfo = $("#div_1");
    DivInfo.empty();
    DivInfo.append('<h1 class="text-center">В этот день нету свободных записей</h1>');
}


function PostRequestForInfoPage(msg) {
    $.ajax({
        type: 'POST',
        url: '../TimeTableDay',
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


function UpdateCalendar() {
    let specSelect = $("#Spec");
    let mounthSelect = $("#Months");
    let Id = specSelect.val();
    let Months = mounthSelect.val();
    $("#div_0").load(`/home/CalendarSegment?Id=${Id}&Mounth=${Months}`);
}
