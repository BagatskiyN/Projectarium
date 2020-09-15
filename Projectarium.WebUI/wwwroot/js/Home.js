
function GetRequestModal(id) {


    const url = '/Home/CreateRequest/' + id;
    //var url = $('#CreateRequestModal').data('url');
    $.get(url, function (data) {
        $("#CreateRequestModal").html(data);
        $("#CreateRequestModal").modal('show');

    })

}
function CreateRequest() {

    const url = '/Home/CreateRequest';
    let item = {
        VacancyId: document.getElementById('RequestVacancyId').value,
        Motivation: document.getElementById('Motivation').value

    }

    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(item),
        traditional: true,
        success: function (data) {
           
        }

    });

}