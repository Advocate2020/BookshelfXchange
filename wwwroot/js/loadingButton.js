$('.btn-process').on('click', function () {
    $(".btn-ring").show();
    $(".btn-process").prop('disabled', true);
    $(".btn-process").val('disabled');
    setTimeout(function () {
        $(".btn-ring").hide();
        $(".btn-process").prop('disabled', false);
    }, 3000);
});