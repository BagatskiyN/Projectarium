﻿function SaveUserData() {
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