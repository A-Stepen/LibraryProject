





$(document).ready(function () {
    $("#RegisterNewBookBtn").click(function () {
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