// Нажатие вкладки Справка в модальном окне //

function Reference(ID) {
    $.ajax({
        url: "/Home/Reference/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//----------------------------------------------------//
//-------Добавление члена комиссии--------------//
function AddKomissiya() {
    $.ajax({
        url: "/Home/AddKomissiya/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления члена комиссии----------//
function KomissiyaSave() {

    var isValid = true;
    if ($('#komis').val() == "") {
        $('#komis').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#komis').css('border-color', 'lightgrey');
    }

    if ($('#Doljnost').val() == "") {
        $('#Doljnost').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Doljnost').css('border-color', 'lightgrey');
    }

    if ($('#FIO').val() == "") {
        $('#FIO').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#FIO').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'location': $('#komis').val(),
        'Nazn': $('#Nazn').val(),
        'Doljnost': $('#Doljnost').val(),
        'FIO': $('#FIO').val()

    };

    $.ajax({
        url: "/Home/KomissiyaSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
// Удаление члена//

function DeletePodpis(ID) {
    $.ajax({
        url: "/Home/DeletePodpis/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
            $('#ServicesModalDelete').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
//Подтверждение удаления члена //
function DeletePodpisOK(ID) {


    $.ajax({
        url: "/Home/DeletePodpisOK/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
// Редактирование члена комиссии //

function KomissiyaEdit(ID) {
    $.ajax({
        url: "/Home/KomissiyaEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования члена комиссии //

function KomissiyaEditSave() {

    var isValid = true;

    if ($('#Doljnost').val() == "") {
        $('#Doljnost').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Doljnost').css('border-color', 'lightgrey');
    }

    if ($('#FIO').val() == "") {
        $('#FIO').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#FIO').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#ID').val(),
        'Nazn': $(Nazn).val(),
        'Doljnost': $('#Doljnost').val(),
        'FIO': $('#FIO').val(),

    };

    $.ajax({
        url: "/Home/KomissiyaEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------Получение градуировочной таблицы в зависимости от резервуара-------------------------

function GetGradTable() {

    var isValid = true;

    if ($('#rezer').val() == "0") {
        $('#rezer').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#rezer').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {

        'rezer': $('#rezer').val(),
        'filial': $('#filial').val(),
    };

    $.ajax({
        url: "/Home/GetGradTable",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#GradTab').html(result);

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//------------------------------------------------------------------------------
// Формирование акта инвентаризации на указанную дату
function LetGradTable() {

    var d = new Date();
    var isValid = true;

    if ($('#filial2').val() == "") {
        $('#filial2').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial2').css('border-color', 'lightgrey');
    }

    if ($('#dataHim').val() == "") 
    {
        $('#dataHim').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#dataHim').css('border-color', 'lightgrey');
    }

    if (parseInt(Date.parse(($('#dataHim').val()))) > parseInt(Date.parse(d)))
    {
        $('#dataHim').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#dataHim').css('border-color', 'lightgrey');
    }
       

    if ($('#dataInvent').val() == "") {
        $('#dataInvent').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#dataInvent').css('border-color', 'lightgrey');
    }

    if (parseInt(Date.parse(($('#dataInvent').val()))) > parseInt(Date.parse(d))) {
        $('#dataInvent').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#dataInvent').css('border-color', 'lightgrey');
    }

    if ($('#timeInvent').val() == "0") {
        $('#timeInvent').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#timeInvent').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {

        'filial2': $('#filial2').val(),
        'dataHim': $('#dataHim').val(),
        'dataInvent': $('#dataInvent').val(),
        'timeInvent': $('#timeInvent').val(),
    };

    $("#search").html("");

    var x = document.getElementById("loadImg");
    x.style.display = "block";

    $.ajax({
        url: "/Home/LetGradTable",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (data) {
            x.style.display = "none";
            $('#Tableinv1').hide();
            $('#Tableinv1').html(data).animate({ opacity: 'show' }, "slow");
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//------------------------------------------------------------------------------

// Удаление строки в градуировочной таблице//

function DeleteGrad(ID) {
    $.ajax({
        url: "/Home/DeleteGrad/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
            $('#ServicesModalDelete').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
//Подтверждение удаления строки в градуировочной таблице //
function DeleteGradOK(ID) {


    $.ajax({
        url: "/Home/DeleteGradOK/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
// Редактирование градуировочной таблицы //

function GradEdit(ID) {
    $.ajax({
        url: "/Home/GradEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования градуировочной таблицы //

function GradEditSave() {

    var isValid = true;

    if ($('#urov').val() == "") {
        $('#urov').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#urov').css('border-color', 'lightgrey');
    }

    if ($('#V').val() == "") {
        $('#V').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#V').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#ID').val(),
        'urov': $('#urov').val(),
        'V': $('#V').val(),

    };

    $.ajax({
        url: "/Home/GradEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//-------Добавление строки в градуировочную таблицу--------------//
function AddGradTable() {

    var isValid = true;
    if ($('#rezer').val() == "0") {
        $('#rezer').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#rezer').css('border-color', 'lightgrey');
    }       

    if (isValid == false) {
        return false;
    }
    $.ajax({
        url: "/Home/AddGradTable/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления строки в градуировочную таблицу----------//
function GradTableSave() {

    var isValid = true;
    if ($('#urov').val() == "") {
        $('#urov').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#urov').css('border-color', 'lightgrey');
    }

    if ($('#V').val() == "") {
        $('#V').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#V').css('border-color', 'lightgrey');
    }
       
    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'IDRez': $('#rezer').val(),
        'urov': $('#urov').val(),
        'V': $('#V').val(),
        
    };

    $.ajax({
        url: "/Home/GradTableSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
//-----------------Получение химанализа в зависимости от резервуара-------------------------

function GetHim() {

    var isValid = true;

    if ($('#filial').val() == "") {
        $('#filial').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial').css('border-color', 'lightgrey');
    }

    if ($('#rezer').val() == "0") {
        $('#rezer').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#rezer').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {
        'filial': $('#filial').val(),
        'rezer': $('#rezer').val(),
        
    };

    $.ajax({
        url: "/Home/GetHim",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            
            $('#res').html(result)
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//------------------------------------------------------------------------------
//-----------------Передача параметров для калькулятора рассчет разности объемов-------------------------

function GetRazn() {

    var isValid = true;

    if ($('#filial1').val() == "") {
        $('#filial1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial1').css('border-color', 'lightgrey');
    }

    if ($('#rezer1').val() == "0") {
        $('#rezer1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#rezer1').css('border-color', 'lightgrey');
    }

    if ($('#Unach').val() == "") {
        $('#Unach').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Unach').css('border-color', 'lightgrey');
    }

    if ($('#Ukon').val() == "") {
        $('#Ukon').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Ukon').css('border-color', 'lightgrey');
    }

    if ($('#Pnach').val() == "") {
        $('#Pnach').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Pnach').css('border-color', 'lightgrey');
    }

    if ($('#Pkon').val() == "") {
        $('#Pkon').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Pkon').css('border-color', 'lightgrey');
    }

    if ($('#Tnach').val() == "") {
        $('#Tnach').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Tnach').css('border-color', 'lightgrey');
    }

    if ($('#Tkon').val() == "") {
        $('#Tkon').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Tkon').css('border-color', 'lightgrey');
    }

    if ($('#Bal').val() == "") {
        $('#Bal').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Bal').css('border-color', 'lightgrey');
    }

    if ($('#Kst').val() == "") {
        $('#Kst').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Kst').css('border-color', 'lightgrey');
    }

    if ($('#Ksr').val() == "") {
        $('#Ksr').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Ksr').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {
        'filial1': $('#filial1').val(),
        'rezer1': $('#rezer1').val(),
        'Unach': $('#Unach').val(),
        'Ukon': $('#Ukon').val(),
        'UnachH2O': $('#UnachH2O').val(),
        'UkonH2O': $('#UkonH2O').val(),
        'Pnach': $('#Pnach').val(),
        'Pkon': $('#Pkon').val(),
        'Tnach': $('#Tnach').val(),
        'Tkon': $('#Tkon').val(),
        'Bal': $('#Bal').val(),
        'Kst': $('#Kst').val(),
        'Ksr': $('#Ksr').val(),

    };

    $.ajax({
        url: "/Home/GetRazn",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            
            $('#razn').html(result)
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//------------------------------------------------------------------------------

//-----------------Передача параметров для калькулятора рассчет по массе-------------------------

function GetRazn1() {

    var isValid = true;

    if ($('#filial2').val() == "") {
        $('#filial2').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial2').css('border-color', 'lightgrey');
    }

    if ($('#rezer2').val() == "0") {
        $('#rezer2').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#rezer2').css('border-color', 'lightgrey');
    }

    if ($('#Unach1').val() == "") {
        $('#Unach1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Unach1').css('border-color', 'lightgrey');
    }

    if ($('#RM1').val() == "") {
        $('#RM1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#RM1').css('border-color', 'lightgrey');
    }

    if ($('#Pnach1').val() == "") {
        $('#Pnach1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Pnach1').css('border-color', 'lightgrey');
    }

    if ($('#Pkon1').val() == "") {
        $('#Pkon1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Pkon1').css('border-color', 'lightgrey');
    }

    if ($('#Tnach1').val() == "") {
        $('#Tnach1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Tnach1').css('border-color', 'lightgrey');
    }

    if ($('#Tkon1').val() == "") {
        $('#Tkon1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Tkon1').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {
        'filial2': $('#filial2').val(),
        'rezer2': $('#rezer2').val(),
        'Unach1': $('#Unach1').val(),
        'UnachH2O1': $('#UnachH2O1').val(),
        'UkonH2O1': $('#UkonH2O1').val(),
        'RM1': $('#RM1').val(),
        'Pnach1': $('#Pnach1').val(),
        'Pkon1': $('#Pkon1').val(),
        'Tnach1': $('#Tnach1').val(),
        'Tkon1': $('#Tkon1').val(),
        'Bal1': $('#Bal1').val(),
        'Kst1': $('#Kst1').val(),
        'Ksr1': $('#Ksr1').val(),
    };

    $.ajax({
        url: "/Home/GetRazn1",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {

            $('#razn1').html(result)
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//------------------------------------------------------------------------------
//--------------Получение таблицы инвентаризации в зависимости от даты и филиала----------------------------------------------

function GetTableInv() {

    var isValid = true;

    if ($('#filial1').val() == "0") {
        $('#filial1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial1').css('border-color', 'lightgrey');
    }

    if ($('#datinv').val() == "0") {
        $('#datinv').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#datinv').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {

        'filial1': $('#filial1').val(),
        'datinv': $('#datinv').val(),
    };

    $.ajax({
        url: "/Home/GetTableInv",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#Tableinv').html(result);

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
 }
//-----Формирование отчета в EXCEL-----------------------------------------------
function Report() {

    var isValid = true;

    if ($('#filial1').val() == "0") {
        $('#filial1').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#filial1').css('border-color', 'lightgrey');
    }

    if ($('#datinv').val() == "0") {
        $('#datinv').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#datinv').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {

        'filial1': $('#filial1').val(),
        'datinv': $('#datinv').val(),
    };

    var stringhref = "/Home/Export?";

    stringhref += "filial1=" + $('#filial1').val() + "&" + "datinv=" + $('#datinv').val();
    window.location = stringhref;
}


//-----------------------------//
// Редактирование инвентаризации //

function InventEdit(ID) {
    $.ajax({
        url: "/Home/InventEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования инвентаризации //

function InventEditSave() {

    var isValid = true;

    if ($('#InvRezerEdit').val() == "") {
        $('#InvRezerEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvRezerEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvUrovEdit').val() == "") {
        $('#InvUrovEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvUrovEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvUrovH2OEdit').val() == "") {
        $('#InvUrovH2OEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvUrovH2OEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvVEdit').val() == "") {
        $('#InvVEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvVEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvVH2OEdit').val() == "") {
        $('#InvVH2OEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvVH2OEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvVNeftEdit').val() == "") {
        $('#InvVNeftEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvVNeftEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvTempEdit').val() == "") {
        $('#InvTempEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvTempEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvPEdit').val() == "") {
        $('#InvPEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvPEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvMassaBruttoEdit').val() == "") {
        $('#InvMassaBruttoEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvMassaBruttoEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvH2OEdit').val() == "") {
        $('#InvH2OEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvH2OEdit').css('border-color', 'lightgrey');
    }


    if ($('#InvSaltEdit').val() == "") {
        $('#InvSaltEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvSaltEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvMehEdit').val() == "") {
        $('#InvMehEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvMehEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvBalProcEdit').val() == "") {
        $('#InvBalProcEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvBalProcEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvBalTonnEdit').val() == "") {
        $('#InvBalTonnEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvBalTonnEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvMassaNettoEdit').val() == "") {
        $('#InvMassaNettoEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvMassaNettoEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvHMimEdit').val() == "") {
        $('#InvHMimEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvHMimEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvVMinEdit').val() == "") {
        $('#InvVMinEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvVMinEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvMNettoMinEdit').val() == "") {
        $('#InvMNettoMinEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvMNettoMinEdit').css('border-color', 'lightgrey');
    }

    if ($('#InvVTehEdit').val() == "") {
        $('#InvVTehEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#InvVTehEdit').css('border-color', 'lightgrey');
    }

    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#InvIDEdit').val(),
        'InvRezer': $('#InvRezerEdit').val(),
        'InvUrov': $('#InvUrovEdit').val(),
        'InvUrovH2O': $('#InvUrovH2OEdit').val(),
        'InvV': $('#InvVEdit').val(),
        'InvVH2O': $('#InvVH2OEdit').val(),
        'InvVNeft': $('#InvVNeftEdit').val(),
        'InvTemp': $('#InvTempEdit').val(),
        'InvP': $('#InvPEdit').val(),
        'InvMassaBrutto': $('#InvMassaBruttoEdit').val(),
        'InvH2O': $('#InvH2OEdit').val(),
        'InvSalt': $('#InvSaltEdit').val(),
        'InvMeh': $('#InvMehEdit').val(),
        'InvBalProc': $('#InvBalProcEdit').val(),
        'InvBalTonn': $('#InvBalTonnEdit').val(),
        'InvMassaNetto': $('#InvMassaNettoEdit').val(),
        'InvHMim': $('#InvHMimEdit').val(),
        'InvVMin': $('#InvVMinEdit').val(),
        'InvMNettoMin': $('#InvMNettoMinEdit').val(),
        'InvVTeh': $('#InvVTehEdit').val(),
    };

    $.ajax({
        url: "/Home/InventEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//-Очистка сформированной инвентаризации при нажатии кнопки ОТМЕНА-----------------------------------------------------------------------------

function CleanTankInv() {
    
    $('#Tableinv').html("");
}

//---------------------------------------------------------------------------------------------------------------------------------------------
//-------Добавление нового ресурса хранения нефти--------------//
function AddResursy() {
    $.ajax({
        url: "/Home/AddResursy/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления ресурса хранения нефти----------//
function ResursySave() {

    var isValid = true;
    if ($('#Naimenovanie').val() == "") {
        $('#Naimenovanie').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Naimenovanie').css('border-color', 'lightgrey');
    }

    if ($('#VRes').val() == "") {
        $('#VRes').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VRes').css('border-color', 'lightgrey');
    }

    if ($('#ResFil').val() == "") {
        $('#ResFil').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#ResFil').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'REsFil': $('#ResFil').val(),
        'Naimenovanie': $('#Naimenovanie').val(),
        'VRes': $('#VRes').val()
        
    };

    $.ajax({
        url: "/Home/ResursySave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
// Удаление ресурса хранения нефти//

function DeleteResursy(ID) {
    $.ajax({
        url: "/Home/DeleteResursy/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
            $('#ServicesModalDelete').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
//Подтверждение удаления ресурса хранения нефти //
function DeleteResursyOK(ID) {


    $.ajax({
        url: "/Home/DeleteResursyOK/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
// Редактирование ресурса хранения нефти //

function ResursyEdit(ID) {
    $.ajax({
        url: "/Home/ResursyEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования ресурса хранения нефти//

function ResursyEditSave() {

    var isValid = true;

    if ($('#Naimen').val() == "") {
        $('#Naimen').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Naimen').css('border-color', 'lightgrey');
    }

    if ($('#VResurs').val() == "") {
        $('#VResurs').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VResurs').css('border-color', 'lightgrey');
    }
        

    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#ID').val(),
        'Naimen': $('#Naimen').val(),
        'VResurs': $('#VResurs').val(),
        
    };

    $.ajax({
        url: "/Home/ResursyEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//-----------------Добавление члена комиссии для Мозыря----------------------------------------------

function AddKomissiyaMoz() {
    $.ajax({
        url: "/Home/AddKomissiyaMoz/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления члена комиссии Мозыря----------//
function KomissiyaSaveMoz() {

    var isValid = true;
    
    if ($('#DoljnostM').val() == "") {
        $('#DoljnostM').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#DoljnostM').css('border-color', 'lightgrey');
    }

    if ($('#FIOM').val() == "") {
        $('#FIOM').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#FIOM').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,        
        'NaznM': $('#NaznM').val(),
        'DoljnostM': $('#DoljnostM').val(),
        'FIOM': $('#FIOM').val()

    };

    $.ajax({
        url: "/Home/KomissiyaSaveMoz",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
//-----------------Добавление члена комиссии для Новополоцка----------------------------------------------

function AddKomissiyaNovop() {
    $.ajax({
        url: "/Home/AddKomissiyaNovop/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления члена комиссии Новополоцка----------//
function KomissiyaSaveNovop() {

    var isValid = true;

    if ($('#DoljnostN').val() == "") {
        $('#DoljnostN').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#DoljnostN').css('border-color', 'lightgrey');
    }

    if ($('#FION').val() == "") {
        $('#FION').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#FION').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,        
        'NaznN': $('#NaznN').val(),
        'DoljnostN': $('#DoljnostN').val(),
        'FION': $('#FION').val()

    };

    $.ajax({
        url: "/Home/KomissiyaSaveNovop",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//-------------------------------------------------------------
//-------Добавление нового ресурса хранения нефти ЛПДС Мозырь-//
function AddResursyMoz() {
    $.ajax({
        url: "/Home/AddResursyMoz/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления ресурса хранения нефти----------//
function ResursySaveMoz() {

    var isValid = true;
    if ($('#NaimenovanieM').val() == "") {
        $('#NaimenovanieM').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NaimenovanieM').css('border-color', 'lightgrey');
    }

    if ($('#VResM').val() == "") {
        $('#VResM').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VResM').css('border-color', 'lightgrey');
    }

    
    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,        
        'NaimenovanieM': $('#NaimenovanieM').val(),
        'VResM': $('#VResM').val()

    };

    $.ajax({
        url: "/Home/ResursySaveMoz",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
//-------Добавление нового ресурса хранения нефти ЛПДС Полоцк----//
function AddResursyNovop() {
    $.ajax({
        url: "/Home/AddResursyNovop/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления ресурса хранения нефти----------//
function ResursySaveNovop() {

    var isValid = true;
    if ($('#NaimenovanieN').val() == "") {
        $('#NaimenovanieN').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NaimenovanieN').css('border-color', 'lightgrey');
    }

    if ($('#VResN').val() == "") {
        $('#VResN').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VResN').css('border-color', 'lightgrey');
    }

    
    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        
        'NaimenovanieN': $('#NaimenovanieN').val(),
        'VResN': $('#VResN').val()

    };

    $.ajax({
        url: "/Home/ResursySaveNovop",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//-----------------------------------------------------------//
//-------Добавление нового резервуара Мозыря --------------//
function AddRezerMoz() {
    $.ajax({
        url: "/Home/AddRezerMoz/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления нового резервуара Мозыря----------//
function RezerMozSave() {

    var isValid = true;
    if ($('#tankidMoz').val() == "") {
        $('#tankidMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#tankidMoz').css('border-color', 'lightgrey');
    }

    if ($('#NaimenovanRezerMoz').val() == "") {
        $('#NaimenovanRezerMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NaimenovanRezerMoz').css('border-color', 'lightgrey');
    }

    if ($('#typeMoz').val() == "") {
        $('#typeMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#typeMoz').css('border-color', 'lightgrey');
    }

    if ($('#UrovMinMoz').val() == "") {
        $('#UrovMinMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#UrovMinMoz').css('border-color', 'lightgrey');
    }

    if ($('#VminMoz').val() == "") {
        $('#VminMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VminMoz').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'NaimenovanRezerMoz': $('#NaimenovanRezerMoz').val(),
        'typeMoz': $('#typeMoz').val(),
        'UrovMinMoz': $('#UrovMinMoz').val(),
        'VminMoz': $('#VminMoz').val(),
        'tankidMoz': $('#tankidMoz').val()

    };

    $.ajax({
        url: "/Home/RezerMozSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//

//-------Добавление нового резервуара Новополоцка --------------//
function AddRezerNovop() {
    $.ajax({
        url: "/Home/AddRezerNovop/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления нового резервуара Новополоцка----------//
function RezerNovopSave() {

    var isValid = true;
    if ($('#tankidNovop').val() == "") {
        $('#tankidNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#tankidNovop').css('border-color', 'lightgrey');
    }

    if ($('#NaimenovanRezerNovop').val() == "") {
        $('#NaimenovanRezerNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NaimenovanRezerNovop').css('border-color', 'lightgrey');
    }

    if ($('#typeNovop').val() == "") {
        $('#typeNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#typeNovop').css('border-color', 'lightgrey');
    }

    if ($('#UrovMinNovop').val() == "") {
        $('#UrovMinNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#UrovMinNovop').css('border-color', 'lightgrey');
    }

    if ($('#VminNovop').val() == "") {
        $('#VminNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VminNovop').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'NaimenovanRezerNovop': $('#NaimenovanRezerNovop').val(),
        'typeNovop': $('#typeNovop').val(),
        'UrovMinNovop': $('#UrovMinNovop').val(),
        'VminNovop': $('#VminNovop').val(),
        'tankidNovop': $('#tankidNovop').val()

    };

    $.ajax({
        url: "/Home/RezerNovopSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//


// Удаление Резервуара //

function DeleteRezer(ID) {
    $.ajax({
        url: "/Home/DeleteRezer/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
            $('#ServicesModalDelete').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
//Подтверждение удаления резервуара //
function DeleteRezerOK(ID) {


    $.ajax({
        url: "/Home/DeleteRezerOK/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
// Редактирование резервуара //

function RezerEdit(ID) {
    $.ajax({
        url: "/Home/RezerEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования резервуара//

function RezerEditSave() {

    var isValid = true;

    if ($('#NumberRezEdit').val() == "") {
        $('#NumberRezEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NumberRezEdit').css('border-color', 'lightgrey');
    }

    if ($('#NameRezEdit').val() == "") {
        $('#NameRezEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#NameRezEdit').css('border-color', 'lightgrey');
    }


    if ($('#typeRezEdit').val() == 0) {
        $('#typeRezEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#typeRezEdit').css('border-color', 'lightgrey');
    }

    if ($('#LevelRezEdit').val() == "") {
        $('#LevelRezEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#LevelRezEdit').css('border-color', 'lightgrey');
    }

    if ($('#VRezEdit').val() == "") {
        $('#VRezEdit').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#VRezEdit').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#ID').val(),
        'NumberRezEdit': $('#NumberRezEdit').val(),
        'NameRezEdit': $('#NameRezEdit').val(),
        'typeRezEdit': $('#typeRezEdit').val(),
        'LevelRezEdit': $('#LevelRezEdit').val(),
        'VRezEdit': $('#VRezEdit').val(),
        
    };

    $.ajax({
        url: "/Home/RezerEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//----ДОБАВЛЕНИЯ КОРРЕКТИРОВОК----------------------------------------------------------------------------------
//-------Добавление корректировки Мозыря--------------//
function AddCorrectMoz() {
    $.ajax({
        url: "/Home/AddCorrectMoz/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления корректировки Мозыря----------//
function CorrectMozSave() {

    var isValid = true;
    
    if ($('#tankidMoz').val() == "") {
        $('#tankidMoz').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#tankidMoz').css('border-color', 'lightgrey');
    }


    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'tankidMoz': $('#tankidMoz').val(),
        'UrovCMoz': $('#UrovCMoz').val(),
        'TempCMoz': $('#TempCMoz').val()

    };

    $.ajax({
        url: "/Home/CorrectMozSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
//-------Добавление корректировки Новополоцка--------------//
function AddCorrectNovop() {
    $.ajax({
        url: "/Home/AddCorrectNovop/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}

//-------Сохранение добавления корректировки Новополоцка----------//
function CorrectNovopSave() {

    var isValid = true;

    if ($('#tankidNovop').val() == "") {
        $('#tankidNovop').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#tankidNovop').css('border-color', 'lightgrey');
    }

        
    if (isValid == false) {
        return false;
    }

    var data = {
        //'ID': ID,
        'tankidNovop': $('#tankidNovop').val(),
        'UrovCNovop': $('#UrovCNovop').val(),
        'TempCNovop': $('#TempCNovop').val()

    };

    $.ajax({
        url: "/Home/CorrectNovopSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
//--------------------------------//
// Удаление корректировки//

function DeleteCorrect(ID) {
    $.ajax({
        url: "/Home/DeleteCorrect/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
            $('#ServicesModalDelete').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
//Подтверждение удаления корректировки //
function DeleteCorrectOK(ID) {


    $.ajax({
        url: "/Home/DeleteCorrectOK/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalDeleteContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//-----------------------------//
// Редактирование корректировки //

function CorrectEdit(ID) {
    $.ajax({
        url: "/Home/CorrectEdit/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
            $('#ServicesModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }

    })
}
//-------------------------//
// Сохранение редактирования корректировки//

function CorrectEditSave() {

    var isValid = true;

    if ($('#tankidECor').val() == "") {
        $('#tankidECor').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#tankidECor').css('border-color', 'lightgrey');
    }
       

    if (isValid == false) {
        return false;
    }

    var data = {

        'ID': $('#ID').val(),
        'tankidECor': $('#tankidECor').val(),
        'UrovECor': $('#UrovECor').val(),
        'TempECor': $('#TempECor').val(),
        
    };

    $.ajax({
        url: "/Home/CorrectEditSave",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        data: JSON.stringify(data),
        dataType: "html",
        success: function (result) {
            $('#ServicesModalContent').html(result);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}