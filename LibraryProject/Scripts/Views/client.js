function reloadClientList() {
    var table = $("#ClientTableData");

    $.ajax({
        url: table.attr("data-url"),
        type: "post",
        success: function (data) {
            var rows = [];
            $.each(data, function (key, val) {
                let row = `<tr><td>${val['Id']}</td><td>${val['Surname']}</td><td>${val['Name']}</td><td>${val['PhoneNumber']}</td></tr>`;
                console.log(row);
                rows.push(row);
            });

            table.empty();
            table.append(rows);
        }
    });
}





$(document).ready(function () {
    $("#AddClientBtn").click(function () {
        $.ajax({
            url: $(this).attr("data-url"),
            type: "post",
            data: $("#NewClientForm").serialize(),
            processData: false,
            success: function () {
                $(':input', '#NewClientForm')
                    .val('');
                reloadClientList();
            }
        });
    });

    reloadClientList();
});