var IDOfRecord = ''
Premission=''
$(document).ready(function () {
    NewOrUpdate = '';//new record or old one
    $("#uname").val("hshiftan");
    $("#psw").val("Hs29572");
    //check user authentication
    $("#Login").submit(function () {
        event.preventDefault();
        $("body").css("cursor", "progress");
        CheckUser();
    })
    $("#bodyContainer").hide()
    $("#editDiv").hide();
    $("#ChangesForm").submit(function () {
        event.preventDefault();
        Save();
    }
    );
    $("#newBTN").click(AddChange)
    ajaxCall("GET", "../api/Changes/PermissionsUsers", "", PermissionsUsers, error);


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
//get the users Permissions
function PermissionsUsers(data) {
    console.log(data);
    PermissionsUsersArr = data;
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
        document.getElementById("nameUser").innerHTML = User.FullName;
        $("#Login").hide();
        $("#bodyContainer").show();
        str = "";
        User.ListUsersNames.sort();
        for (var item of User.ListUsersNames) {
            str += "<option>" + item + "</option>"
        }
        document.getElementById("Name").innerHTML = str;
        document.getElementById("NamePrgrammer").innerHTML = str;
        //check if the user has special Permissions-edit/it/finance
        PermissionUser = PermissionsUsersArr.find(p => p.UserName.trim() == User.UserName.trim())
        if (PermissionUser != 'undefined')
            PremissionEdit = false
        else {
            if (PermissionUser.PremissionEdit)
                PremissionEdit = true
            else
                PremissionEdit = false
        }
          

        ajaxCall("GET", "../api/Changes", "", successCBChange, error);
    }

}

//After get changes details from db
function successCBChange(Changes) {
    ChangesFiles = Changes; // keep the cars array in a global variable;
    try {
        IdNext = ChangesFiles[0].Id + 1;//for the next change record
        tbl = $('#ChangesTable').DataTable({
            data: ChangesFiles,
            pageLength: 7,
            columns: [
                {
                    render: function (data, type, row, meta) {//build for any row
                        let dataChange = "data-ID='" + row.Id + "'";
                        viewBtn = "<button type='button' class = 'viewBtn btn btn-info' " + dataChange + "> View </button>";
                        if (User.FullName == row.Programmer || PermissionUser) //if the name of current user match the programmer or if the user has premission
                        {
                            editBtn = "<button type='button' class = 'editBtn btn btn-success' " + dataChange + "> Edit </button>";
                            return editBtn + viewBtn
                        }
                        else
                            return viewBtn
                    }
                },
                { data: "NameInitated" },
                { data: "Programmer" },
                { data: "Subject" },
                { data: "Date" },

                {
                    data: "Initated",
                    render: function (data, type, row, meta) {
                        let color = "red";
                        if (data)
                            color = "green";
                        AprrovedInitated = `<button type='button' style='background-color: ${color}' data-type='Initated' id='Initated-${row.Id}' class='Approved' > ${row.NameInitated} </button >`


                        return AprrovedInitated
                    }
                },
                {
                    data: "It",
                    render: function (data, type, row, meta) {
                        let color = "red";
                        if (data)
                            color = "green";
                        ApprovedIt = `<button type='button' style='background-color: ${color}' data-type='It' id='It-${row.Id}' class='Approved'> IT MANAGER </button>`
                        return ApprovedIt
                    }
                },
                {
                    data: "FinanceApproveNecessary",
                    render: function (data, type, row, meta) {
                        if (data) {
                            let color = "red";
                            if (row.Finance)
                                color = "green";
                            ApprovedFinance = `<button type='button' style='background-color: ${color}' data-type='Finance' id='Finance-${row.Id}' class='Approved' > Finance MANAGER </button >`
                            return ApprovedFinance;
                        }
                        else
                            return null;

                    }
                }

            ],
        });
    }

    catch (err) {
        alert(err);
    }
}

function error(err) {
    swal("Error: " + err);
}


$(document).on("click", ".viewBtn", function () {
    markSelected(this);
    $("#editDiv").show();
    row.className = 'selected';
    $("#editDiv :input").attr("disabled", "disabled"); // view mode: disable all controls in the form
    populateFields(this.getAttribute('data-ID'));
});

// mark the selected row
function markSelected(btn) {
    $("#ChangesTable tr").removeClass("selected"); // remove seleced class from rows that were selected before
    row = (btn.parentNode).parentNode; // button is in TD which is in Row
    row.className = 'selected'; // mark as selected
}

//edit button-relase all inputs
$(document).on("click", ".editBtn", function () {
    NewOrUpdate = "Update"
    markSelected(this);
    $("#editDiv").show();
    $("#editDiv :input").prop("disabled", false); // edit mode: enable all controls in the form
    populateFields(this.getAttribute('data-ID')); // fill the form fields according to the selected row
});

//clicked on signature 
$(document).on("click", ".Approved", function () {
    //recognize which change button
    NewOrUpdate="Update"
    row = (this.parentNode).parentNode; // button is in TD which is in Row
    caseButton = this.getAttribute("data-type")//which type button approve?IT/finance/user
    idChange = this.getAttribute("id");

    //confirm the approval
    Swal.fire({
        title: `Approved by ${caseButton}`,
        text: `Are you sure to improve?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Approve'
    }).then((result) => {
        if (result.value) {
            //check if current user can sign
            ajaxCall("GET", `../api/Changes/User/${User.FullName}/${User.UserName}/${caseButton}/${idChange}`, "", CheckIfcanSign, error);
        }
    })

    

});

//check if current user can sign
function CheckIfcanSign(checked) {
    if (checked) {
        document.getElementById(idChange).style.background = 'green';//change the button to green
        Swal.fire(
            'Approved successfully',
            'success'
        )
        swal("Approve Successfuly!", "Great Job", "success");

    }
        else {
            Swal.fire({
                icon: 'error',
                title: 'שגיאה',
                text: "You do not have permission"
                //footer: '<a href>Why do I have this issue?</a>'
            })
        }
    }


function error(e) {
    alert(e);
}


// fill the form fields
function populateFields(ChangeIDlocal) {
    ChangeID = ChangeIDlocal
    ajaxCall("GET", `../api/Changes/getChange/${ChangeID}`, "", GetDataChange, error);    
}

//get data of change after click on edit/view
function GetDataChange(objectChange) {
    NewOrUpdate = "Update";
    ClearFields();
    $("#Name").val(objectChange.NameInitated);
    $("#NamePrgrammer").val(objectChange.Programmer);
    $("#EmpNum").val(objectChange.ProgrammerNum);
    $("#date").val(objectChange.Date);
    $("#subject").val(objectChange.Subject);
    $("#description").val(objectChange.Description);
    for (var i = 0; i < objectChange.ObjectsList.length; i++) {
        $("#new_libary").val(objectChange.ObjectsList[i].Libary);
        $("#new_object").val(objectChange.ObjectsList[i].Object);
        $("#new_comment").val(objectChange.ObjectsList[i].Comments);
        add_row_updates();
    }
   
    IDOfRecord = `data-IDrecord=${objectChange.Id}`
    
    for (var i = 0; i < objectChange.TestsList.length; i++) {
        $("#descTest").val(objectChange.TestsList[i].Description);
        add_row_test();
        if (objectChange.TestsList[i].ApprovedInitated)
            document.getElementById("UserSign"+(i+1)).checked = true;
        if (objectChange.TestsList[i].ApprovedProgrammer)
            document.getElementById("ProgSign" + (i + 1)).checked = true;        
    }

}

function AddChange() {
    ClearFields();
    IDOfRecord = `data-IDrecord=${IdNext}`
    NewOrUpdate = "New";
    $("#editDiv").show();

}
//add or update change on as400
function onSubmitFunc() {
    return false;
}


function Save() {

        //pick the table of objects affected
        var tableObjects = document.getElementById("data_table");
        arrObject = [];
        for (var i = 1, row; row = tableObjects.rows[i]; i++) {
            var elementExists = document.getElementById("libary" + i);
            if (elementExists != null) {
                var obj = {
                    Libary: row.cells.namedItem("libary" + i).innerHTML,
                    Object: row.cells.namedItem("object" + i).innerHTML,
                    Comments: row.cells.namedItem("commentObj" + i).innerHTML
                }
                arrObject.push(obj);
            }

        }

        //pick the table of tests
        var tableObjects = document.getElementById("tableTestingPlan");
        arrTest = [];
        for (var i = 1, row; row = tableObjects.rows[i]; i++) {
            var elementTestExists = document.getElementById("descTest" + i);
            if (elementTestExists != null) {

                var obj = {
                    Numerator: row.cells.namedItem("No" + i).innerHTML,
                    Description: row.cells.namedItem("descTest" + i).innerHTML,
                    ApprovedInitated: document.getElementById("UserSign" + i).checked,
                    ApprovedProgrammer: document.getElementById("ProgSign" + i).checked
                }
                arrTest.push(obj);
            }
        }

    if (NewOrUpdate == "Update") {
        IDcurrent = ChangeID;//comes from the element chosen
        ajaxType = "PUT"
    } 
    else if (NewOrUpdate == "New") {
        IDcurrent = IdNext;//new one
        ajaxType="POST"
    }

        var ChangeObj = {
            Id: IDcurrent,
            NameInitated: $("#Name").val(),
            Programmer: $("#NamePrgrammer").val(),
            ProgrammerNum: $("#EmpNum").val(),
            Date: $("#date").val(),
            Subject: $("#subject").val(),
            Description: $("#description").val(),
            FinanceApproveNecessary: $("#automatic").is(':checked'),
            ObjectsList: arrObject,
            TestsList: arrTest
    }

    ajaxCall(ajaxType, "../api/Changes", JSON.stringify(ChangeObj), postChangeSuccess, postChangeError);//update records
    
}



function postChangeSuccess(massege) {
    if (massege == "Success") {
        if (NewOrUpdate == "New")
            swal("Change was added successfully!", "", "success");
        else
            swal("Change was updated successfully!", "", "success");
        ajaxCall("GET", "../api/Changes", "", successCBChange, error);
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'שגיאה',
            text: massege
            //footer: '<a href>Why do I have this issue?</a>'
        })
    }


}

function postChangeError(massege) {
    alert("שגיאה");
}
function ClearFields() {
    $("#Name").val('');
    $("#EmpNum").val('');
    $("#date").val('');
    $("#file").val('');
    $("#libary").val('');
    $("#subject").val('');
    $("#description").val('');
    $("#data_table").val('');
    $("#descTest").val('');
}


function edit_row(no) {
    document.getElementById("edit_button" + no).style.display = "none";
    document.getElementById("save_button" + no).style.display = "block";

    var name = document.getElementById("libary" + no);
    var country = document.getElementById("country_row" + no);
    var age = document.getElementById("age_row" + no);

    var name_data = name.innerHTML;
    var country_data = country.innerHTML;
    var age_data = age.innerHTML;

    name.innerHTML = "<input type='text' id='name_text" + no + "' value='" + name_data + "'>";
    country.innerHTML = "<input type='text' id='country_text" + no + "' value='" + country_data + "'>";
    age.innerHTML = "<input type='text' id='age_text" + no + "' value='" + age_data + "'>";
}

function save_row(no) {
    var name_val = document.getElementById("name_text" + no).value;
    var country_val = document.getElementById("country_text" + no).value;
    var age_val = document.getElementById("age_text" + no).value;

    document.getElementById("libary" + no).innerHTML = name_val;
    document.getElementById("country_row" + no).innerHTML = country_val;
    document.getElementById("age_row" + no).innerHTML = age_val;

    document.getElementById("edit_button" + no).style.display = "block";
    document.getElementById("save_button" + no).style.display = "none";
}

function delete_row(no) {
    document.getElementById("row" + no + "").outerHTML = "";
}
//ADD row for libary and Objects 
function add_row_updates() {
    var new_libary = document.getElementById("new_libary").value;
    var new_object = document.getElementById("new_object").value;
    var new_comment = document.getElementById("new_comment").value;

    var table = document.getElementById("data_table");
    var table_len = (table.rows.length) - 1;
    var row = table.insertRow(table_len).outerHTML = "<tr id='row" + table_len + "'><td id='libary" + table_len + "'>" + new_libary + "</td><td id='object" + table_len + "'>" + new_object + "</td><td id='commentObj" + table_len + "'>" + new_comment + "</td><td><input type='button' id='edit_button" + table_len + "' value='Edit' class='edit' onclick='edit_row(" + table_len + ")'> <input type='button' id='save_button" + table_len + "' value='Save' class='save' onclick='save_row(" + table_len + ")'> <input type='button' value='Delete' class='delete' onclick='delete_row(" + table_len + ")'></td></tr>";

    document.getElementById("new_libary").value = "";
    document.getElementById("new_object").value = "";
    document.getElementById("new_comment").value = "";
}
//ADD row for testing 
function add_row_test() {
    var table = document.getElementById("tableTestingPlan");
    var descTest = document.getElementById("descTest").value;
    if (document.getElementById("SignUser").checked)
        SignUser = "checked";
    else
        SignUser = "";
    if (document.getElementById("SignDeveloper").checked)
        SignDeveloper = "checked";
    else
        SignDeveloper = "";

    var table = document.getElementById("tableTestingPlan");
    var table_len = (table.rows.length) - 1;
    if (NewOrUpdate == "New") {//disable checkbox of user sign because the programmer open the new change
        Premission = 'onclick="return false"'
    }
    var row = table.insertRow(table_len).outerHTML = "<tr id='row" + table_len + "'" + IDOfRecord + " > <td id='No" + table_len + "' disabled>" + table_len + "</td> <td id='descTest" + table_len + "'>" + descTest + "</td> <td><input type='checkbox' id='UserSign" + table_len + "' class='center-block form-control' " + SignUser + Premission+"/></td></td > <td><input type='checkbox' id='ProgSign" + table_len + "' class='center-block form-control' " + SignDeveloper + " /></td> <td><input type='button' id='edit_button" + table_len + "' value='Edit' class='edit' onclick='edit_row(" + table_len + ")'> <input type='button' id='save_button" + table_len + "' value='Save' class='save' onclick='save_row(" + table_len + ")'> <input type='button' value='Delete' class='delete' onclick='delete_row(" + table_len + ")'></td></tr>";

    document.getElementById("No").value = table_len + 1;
    document.getElementById("descTest").value = "";
    document.getElementById("SignUser").checked = false;
    document.getElementById("SignDeveloper").checked = false;

}

//check if user can sign on the test


