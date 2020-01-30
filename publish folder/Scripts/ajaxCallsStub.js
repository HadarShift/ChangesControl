Cars = [
     {
         Id : 1000,
         Model: "3",
         Manufacturer: "Mazda",
         Price: 100000,
         Year: 2017,
         Hand: 1,
         Color: "White",
         Automatic: true,
         Image: "Car1.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
         
     },
     {
         Id: 1001,
         Model: "3",
         Manufacturer: "Mazda",
         Price: 90000,
         Year: 2016,
         Hand: 2,
         Color: "White",
         Automatic: true,
         Image: "Car3.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
     },
     {
         Id: 1002,
         Model: "6",
         Manufacturer: "Mazda",
         Price: 110000,
         Year: 2016,
         Hand: 2,
         Color: "Black",
         Automatic: false,
         Image: "Car2.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
     },
     {
         Id: 1003,
         Model: "6",
         Manufacturer: "Mazda",
         Price: 110000,
         Year: 2016,
         Hand: 2,
         Color: "red",
         Automatic: false,
         Image: "Car1.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
     },
     {
         Id: 1004,
         Model: "Corola",
         Manufacturer: "Toyota",
         Price: 110000,
         Year: 2016,
         Hand: 3,
         Color: "White",
         Automatic: true,
         Image: "Car3.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
     },
     {
         Id: 1005,
         Model: "Corola",
         Manufacturer: "Toyota",
         Price: 120000,
         Year: 2017,
         Hand: 2,
         Color: "White",
         Automatic: true,
         Image: "Car2.jpg",
         Description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
     }
];

Changes = [
    {
        ID: 1,
        Name: "Hadar",
        EmployeeNum: 19474,
        Date: "29-05-2019",
        File: "mcovip",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    },
    {
        ID: 2,
        Name: "Yafa",
        EmployeeNum: 19473,
        Date: "29-05-2019",
        File: "ith",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    }, {
        ID: 1,
        Name: "Majd",
        EmployeeNum: 19474,
        Date: "29-05-2019",
        File: "mcovip",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    }, {
        ID: 1,
        Name: "Hadar",
        EmployeeNum: 19474,
        Date: "29-05-2019",
        File: "mcovip",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    }, {
        ID: 1,
        Name: "Hadar",
        EmployeeNum: 19474,
        Date: "29-05-2019",
        File: "mcovip",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    }, {
        ID: 1,
        Name: "Hadar",
        EmployeeNum: 19474,
        Date: "29-05-2019",
        File: "mcovip",
        Libary: "tapiali",
        Subject: "שינוי כרטיס"
    }

];


function ajaxCall(method, api, data, successCB, errorCB) {
    //$.ajax({
    //    type: method,
    //    url: api,
    //    data: data,
    //    cache: false,
    //    contentType: "application/json",
    //    dataType: "json",
    //    success: successCB,
    //    error: errorCB
    //});

    if (method == "GET" && api == "../api/cars") {
        successCB(Cars);
        return;
    }

    if (method == "GET" && api == "../api/changes") {
        successCBChange(Changes);
    }

    else if (method == "PUT" && api == "../api/cars") {
        let car = JSON.parse(data);
        for (var i = 0; i < Cars.length; i++) {
            if (Cars[i].Id == car.Id) {
                Cars[i] = car;
                
                successCB(Cars);
                return;
            }
        }

        errorCB("did not manage to update the DB");
    }


    else if (method == "POST" && api == "../api/cars") {
        let car = JSON.parse(data);
        car.Id = getMaxId(Cars) + 1;
        Cars.push(car);
        successCB(Cars);
        //errorCB("did not manage to insert the new car into the DB");
    }

    else if (method == "DELETE") {
        splitArr = api.split('/');
        let id = splitArr[splitArr.length - 1];
        temp = [];
        index = 0;
        for (var i = 0; i < Cars.length; i++) {
            if (parseInt(Cars[i].Id) != id) {
                temp[index] = Cars[i];
                index++;
            }
        }
        Cars = temp;
        successCB(Cars);
    }


}

function getMaxId(cars){
    let max = 0;
    for (var i = 0; i < cars.length; i++) {
        if (cars[i].Id > max)
            max = cars[i].Id;
    }
    return max;
}