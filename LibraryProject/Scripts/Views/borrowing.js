var searchAjax = null;
var filterEditTimeout = null;


function fillFilteredTable(data) {
    var table = $("#FilteredBooksTable");

    var rows = [];
    $.each(data, function (key, val) {
        let row = `<tr class="BookRow"><td hidden="hidden">${val['Id']}</td><td>${val['Title']}</td><td>${val['Author']}</td><td>${val['Description']}</td></tr>`;
        rows.push(row);
    });
    table.empty();
    table.append(rows);
}

function updateBookList() {
    var titleFilter = $("#FilterTitle").val();
    var authorFilter = $("#FilterAuthor").val();

    if (searchAjax !== null) {
        searchAjax.abort();
    }
    searchAjax = $.ajax({
        url: $("#FilterForm").attr("data-url"),
        type: "post",
        data: $("#FilterForm").serialize(),
        processData: false,
        success: fillFilteredTable
    });
}

function selectBook(id) {
    $.ajax({
        url: $("#BookConfirmation").attr("data-url"),
        type: "post",
        data: JSON.stringify({ "id": id }),
        contentType: "application/json",
        success: function (data) {
            $("#BookSelectionForm").hide();
            $("#BookConfirmation").show();

            $("#bookConfirmId").text(data["Id"]);
            $("#bookConfirmTitle").text(data["Title"]);
            $("#bookConfirmAuthor").text(data["Author"]);
            //$("#bookConfirmQuantity").innerText = data["id"];
        }
    });
}



$(document).ready(function () {
    $("input[name$='Filter']").on('input', function () {
        if (filterEditTimeout !== null) {
            clearTimeout(filterEditTimeout);
        }
        filterEditTimeout = setTimeout(updateBookList, 500);
    });

    $("#FilteredBooksTable").on('click', '.BookRow', function () {
        var id = parseInt($(this).children()[0].innerText);
        selectBook(id);
    });
});