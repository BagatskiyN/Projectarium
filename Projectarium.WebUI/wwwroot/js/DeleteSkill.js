function DeleteSkill(text) {
    const url = '/AdminUsers/DeleteSkill';
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(text),
        traditional: true,

        success: function (data) {
            $("#Skills").replaceWith(data);
        }
    });
}