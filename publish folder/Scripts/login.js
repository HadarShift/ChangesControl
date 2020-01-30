var IDOfRecord = ''
Premission = ''
local = false;//change also in JavaScript.js
$(document).ready(function () {
    //$("#uname").val("hshiftan");
    //$("#psw").val("Hs29573");
    //check user authentication
    $("#Login").submit(function () {
        event.preventDefault();
        $("body").css("cursor", "progress");
        CheckUser();
    })

    //to know the api address depend on local or not
    if (local) {
        apiStartString = ".."
    }
    else {
        apiStartString = "http://alwebs/Changes"
    }
});

//check user authentication
function CheckUser() {
    //get the users Permissions
    userData = {
        UserName: $("#uname").val(),
        Password: $("#psw").val()
    }
    ajaxCall("POST", `${apiStartString}/api/Changes/User`, JSON.stringify(userData), successValidationUser, error);

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
        console.log(localStorage["User"])
        //move to the main page
        if (local) {
            window.location.href = "http://localhost:54539/Pages/HtmlPage1.html";//local
            console.log("http://localhost:54539/Pages/HtmlPage1.html")
        }
        else {
            window.location.href = "http://alwebs/Changes/Pages/HtmlPage1.html";
            console.log("http://alwebs/Changes/Pages/HtmlPage1.html")
        }

    }
}
function error(err) {
    swal("Error: " + err);
}