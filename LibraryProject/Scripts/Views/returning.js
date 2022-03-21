var searchAjax = null;
var filterEditTimeout = null;


function fillFilteredBorrowTable(data) {
    var table = $("#FilteredBorrowedTabe");

    var rows = [];
    $.each(data, function (key, val) {
        let row = `<tr class="BorrowRow"><td hidden="hidden">${val['Id']}</td><td>${val['ClientSurname']}</td><td>${val['ClientName']}</td><td>${val['ClientPhone']}</td><td>${val['BookTitle']}</td><td>${val['BookAuthor']}</td></tr>`;
        rows.push(row);
    });
    table.empty();
    table.append(rows);

}


function updateBorrowTable() {
    if (searchAjax !== null) {
        searchAjax.abort();
    }
    searchAjax = $.ajax({
        url: $("#BorrowFilterForm").attr("data-url"),
        type: "post",
        data: $("#BorrowFilterForm").serialize(),
        processData: false,
        success: fillFilteredBorrowTable
    });
}


function selectBorrowing(id) {
    $.ajax({
        url: $("#submitForm").attr("data-url"),
        type: "post",
        data: JSON.stringify({ "id": id }),
        contentType: "application/json",
        processData: false,
        success: function (data) {
            $.each(data, function (key, val) {
                $("[name=" + key + "]").val(val);
            })
            $("#submitForm").show();
            $("#BorrowFilterForm").hide();
        }
    });

}

$(document).ready(function () {
    $("input.Filter").on("input", function () {
        if (filterEditTimeout !== null) {
            clearTimeout(filterEditTimeout);
        }
        filterEditTimeout = setTimeout(updateBorrowTable, 500);
    });

    $("#FilteredBorrowedTabe").on('click', '.BorrowRow', function () {
        var id = parseInt($(this).children()[0].innerText);
        selectBorrowing(id);
    });
});