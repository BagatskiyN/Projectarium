//$(document).ready(
//    document.querySelectorAll("#BtnAddVacancy").forEach(elem => {

//        elem.addEventListener("click", (event) => { console.log(event.target) })
//    })
//  )
function NewVac() {
    const url = '/ProjectManager/AddVacancy';
    title = document.getElementById('TechTitle').value;
    let projectId = document.getElementById('ProjectId').value;
    let item = {
        ProjectId: projectId,
        VacancyTitle: title
    }
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(item),
        traditional: true,

        success: function (data) {
            $("#Vac").append(data);
        }
    });

}



function CreateProject() {
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

};

function AddSkill(e, id) {
    console.log(e.previousElementSibling.value);
    
    const url = '/ProjectManager/AddVacancySkill';
    let SkillTitle = e.previousElementSibling.value;
    let item = { text: SkillTitle, id: id }
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(item),
        traditional: true,

        success: function (data) {
            let elementName = "#Skills"+ id;
            $(elementName).append(data);
        }
    });

}

function DeleteSkill(id) {

    const url = '/ProjectManager/DeleteSkill'; 

    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(id),
        traditional: true,

        success: function (data) {
            let elemName = "Skill" + id;
            let elem = document.getElementById(elemName);
            console.log(elem);
            elem.remove();

           
        }
    });

}
function SaveVacancyDescription(e,id) {
    const url = '/ProjectManager/SaveVacancyDescription';
    let elem = e.previousElementSibling;
    let NewDescription = { Id: id, Description: elem.firstElementChild.value }
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(NewDescription),
        traditional: true,


    });
   
}
function DeleteVacancy(e,id) {
    const url = '/ProjectManager/DeleteVacancy';
   
    
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(id),
        traditional: true,
        success: function (data) {
            e.parentElement.parentElement.remove();
        }
    });

}


async function AddLink() {
    const url = '/ProjectManager/AddLink';

    title = document.getElementById('LinkText').value;
    let projectId = document.getElementById('ProjectId').value;
    let item = {
        ProjectId: projectId,
        Link: title
    }
    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(item),
            credentials: 'include',
            headers: {
                "XSRF-TOKEN": xsrf_token, 'Accept': 'application/json; charset=utf-8',
                'Content-Type': 'application/json;charset=UTF-8' }
        })
        if (response.status === 200) {
            const text = await response.text();
            $("#Links").append(text);
        }
      
    } catch (error) {
        console.error('Ошибка добавления ссылки');
    }
}
async function DeleteLink(e,id) {
    const url = '/ProjectManager/DeleteLink';
    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(id),
            credentials: 'include',
            headers: {
                "XSRF-TOKEN": xsrf_token, 'Accept': 'application/json; charset=utf-8',
                'Content-Type': 'application/json;charset=UTF-8'
            }
        })
        if (response.status === 200) {
            console.log(e.parentElement);
           e.parentElement.remove();
         
        }

    } catch (error) {
        console.error('Ошибка удаления ссылки');
    }
}