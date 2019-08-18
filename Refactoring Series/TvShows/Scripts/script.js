$(document).ready(function () {
    $.ajax({
        url: '/Home/Carousel',
        type: 'GET',
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);

        },
        success: function (result) {

            $('#carousel').html(result);
        }
    });
});