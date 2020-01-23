var IDOfRecord = ''
Premission = ''
$(document).ready(function () {
    $("#uname").val("hshiftan");
    $("#psw").val("Hs29573");
    //check user authentication
    $("#Login").submit(function () {
        event.preventDefault();
        $("body").css("cursor", "progress");
        CheckUser();
    })

});

//check user authentication
function CheckUser() {
    //get the users Permissions
    userData = {
        UserName: $("#uname").val(),
        Password: $("#psw").val()
    }
    ajaxCall("POST", "../api/Changes/User", JSON.stringify(userData), successValidationUser, error);

}

//return user details after validation
function successValidationUser(data) {
    $("body").css("cursor", "default");
    User = data;//user details
    console.log(data);
    if (!User.IsValid) {//wrong login
        Swal.fire({
            icon: 'error',
            title: 'שגיאה',
            text: "username or password is incorrect"
            //footer: '<a href>Why do I have this issue?</a>'
        })
    }
    //good login
    else {
        localStorage["User"] = JSON.stringify(User);//save the user details in local storage
        window.location.href = "http://localhost:54539/Pages/HtmlPage1.html";

    }
}
function error(err) {
    swal("Error: " + err);
}