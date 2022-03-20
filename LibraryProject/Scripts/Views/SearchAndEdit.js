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

function editBook(id) {
    $.ajax({
        url: $("#BookEditForm").attr("data-url"),
        type: "post",
        data: JSON.stringify({ "id": id }),
        contentType: "application/json",
        success: function (data) {
            $("input[name$='Id']").val(id);
            $("input[name$='Title']").val(data["Title"]);
            $("input[name$='Author']").val(data["Author"]);
            $("input[name$='Description']").val(data["Description"]);

            $("#BookEditForm").show();
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
        editBook(id);
    });

    $("#ApplyEditForm").click(function () {
        $.ajax({
            url: $(this).attr("data-url"),
            type: "post",
            data: $("#BookEditForm").serialize(),
            processData: false,
            success: function () {
                $('#BookEditForm').hide();
            }

        });
    });
});