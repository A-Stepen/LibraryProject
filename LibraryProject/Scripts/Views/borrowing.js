var searchAjax = null;
var filterEditTimeout = null;


function fillFilteredBookTable(data) {
    var table = $("#FilteredBooksTable");

    var rows = [];
    $.each(data, function (key, val) {
        let row = `<tr class="BookRow"><td hidden="hidden">${val['Id']}</td><td>${val['Title']}</td><td>${val['Author']}</td><td>${val['Description']}</td></tr>`;
        rows.push(row);
    });
    table.empty();
    table.append(rows);
}


function fillFilteredClientTable(data) {
    var table = $("#FilteredClientTable");

    var rows = [];
    $.each(data, function (key, val) {
        let row = `<tr class="ClientRow"><td hidden="hidden">${val['Id']}</td><td>${val['Name']}</td><td>${val['Surname']}</td><td>${val['PhoneNumber']}</td></tr>`;
        rows.push(row);
    });
    table.empty();
    table.append(rows);
}


function updateBookList() {
    if (searchAjax !== null) {
        searchAjax.abort();
    }
    searchAjax = $.ajax({
        url: $("#BookFilterForm").attr("data-url"),
        type: "post",
        data: $("#BookFilterForm").serialize(),
        processData: false,
        success: fillFilteredBookTable
    });
}


function updateClientList() {
    if (searchAjax !== null) {
        searchAjax.abort();
    }
    searchAjax = $.ajax({
        url: $("#ClientSelectionForm").attr("data-url"),
        type: "post",
        data: $("#ClientFilterForm").serialize(),
        processData: false,
        success: fillFilteredClientTable
    });
}

function selectBook(id) {
    $.ajax({
        url: $("#BookConfirmation").attr("data-url"),
        type: "post",
        data: JSON.stringify({ "propertyId": id }),
        contentType: "application/json",
        success: function (data) {
            var available = data["Item2"];
            $("#bookConfirmId").text(data["Item2"]);
            $("#bookConfirmTitle").text(data["Item1"]["Title"]);
            $("#bookConfirmAuthor").text(data["Item1"]["Author"]);
            $("#bookConfirmQuantity").text(data["Item3"]);

            $("#BookSelectionForm").hide();
            $("#BookConfirmation").show();

            if (available > 0) {
                $("#ClientSelectionForm").show();
            }

        }
    });
}



$(document).ready(function () {
    $("input.BookFilter[name$='Filter']").on('input', function () {
        if (filterEditTimeout !== null) {
            clearTimeout(filterEditTimeout);
        }
        filterEditTimeout = setTimeout(updateBookList, 500);
    });

    $("#FilteredBooksTable").on('click', '.BookRow', function () {
        var id = parseInt($(this).children()[0].innerText);
        selectBook(id);
    });

    $("input.ClientFilter[name$='Filter']").on('input', function () {
        if (filterEditTimeout !== null) {
            clearTimeout(filterEditTimeout);
        }
        filterEditTimeout = setTimeout(updateClientList, 500);
    });

    $("#FilteredClientTable").on('click', '.ClientRow', function () {
        var id = parseInt($(this).children()[0].innerText);
        $("#ClientSelectionForm").hide();
        $("#BorowingConfirmation").show();

        $("input#clientId").val($(this).children()[0].innerText);
        $("input#bookId").val($("#bookConfirmId").text());
        $("div#clientName").text($(this).children()[1].innerText);
        $("div#clientSurname").text($(this).children()[2].innerText);
        $("div#clientPhone").text($(this).children()[3].innerText);
    });
});