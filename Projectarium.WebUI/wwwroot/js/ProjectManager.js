function NewVac() {
    const url = '/ProjectManager/AddVacancy';
    title = document.getElementById('TechTitle').value;
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(title),
        traditional: true,

        success: function (data) {
            $("#Vac").replaceWith(data);
        }
    });

}

function CreateProject()
    {
    const url = '/ProjectManager/Create';
    ProjectTitle = document.getElementById('ProjectTitle').value;
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(ProjectTitle),
        traditional: true,

        success: function (data) {
            location.reload()
        }
    });
    
}
<script type="text/javascript">
    $(document).ready(
        function () {
        $('.TextAreaAboutProject').click(function () {

            var url = '/ProjectManager/SaveProjectDescription';
            $.ajax(url, function (data) {
                $("#CreateModal").html(data);
                $("#CreateModal").modal('show');

            })

        })
        });


</script>