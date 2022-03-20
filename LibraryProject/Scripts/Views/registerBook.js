





$(document).ready(function () {
    $("#RegisterNewBookBtn").click(function () {
        alert("Button click");
        $.ajax({
            url: $(this).attr("data-url"),
            type: "post",
            data: $("#BookForm").serialize(),
            processData: false,
            success: function () {
                $(':input', '#BookForm')
                    .val('');
            }
        });
    });
});