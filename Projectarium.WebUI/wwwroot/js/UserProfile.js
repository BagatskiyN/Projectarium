function SaveUserData() {
    let UserName = document.getElementById('UserName').value;
    let AboutUser = document.getElementById('AboutUser').value;
    let Image = document.getElementById('imgInput').files[0];
    let element = {
        UserName: UserName,
        AboutUser: AboutUser,
        Image: Image
    };
    var formData = new FormData();
    formData.append("UserName", UserName);
    formData.append("AboutUser", AboutUser);
    formData.append("Image", Image);
   
    console.log(document.getElementById('imgInput'));
    const url = '/UserProfile/Edit'; 
    $.ajax(url, {
        method: 'post',
        processData: false,
        contentType: false,

        data: formData,
        traditional: true,





    });
}
function AddSkillUserProfile()
{
 

    const url = '/UserProfile/AddUserSkill';
    title = document.getElementById('SkillText').value;
    
    $.ajax(url, {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(title),
        traditional: true,

        success: function (data) {
            let elementName = "#Skills";
            $(elementName).append(data);
            if (document.getElementById('NoSkill') != null) {
                document.getElementById('NoSkill').remove();
            }
        }
    });

}
async function AddUserLink() {
    const url = '/UserProfile/AddUserLink';

    title = document.getElementById('LinkText').value;

    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(title),
            credentials: 'include',
            headers: {
                "XSRF-TOKEN": xsrf_token, 'Accept': 'application/json; charset=utf-8',
                'Content-Type': 'application/json;charset=UTF-8'
            }
        })
        if (response.status === 200) {
            const text = await response.text();
            $("#Links").append(text);
            if (document.getElementById('NoLink') != null) {
                document.getElementById('NoLink').remove();
            }
        }

    } catch (error) {
        console.error('Ошибка добавления ссылки');
    }
}