$(document).ready(function () {
    //Layout
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

    //Home
    size_td = $("#SeenSeriesList td").length;
    $("#SeenSeriesList td").hide();
    x = 6;
    $('#SeenSeriesList td:lt(' + x + ')').show();

    $('#showMore').click(function () {
        x = (x + 3 <= size_td) ? x + 3 : size_td;
        $('#SeenSeriesList td:lt(' + x + ')').show();
    });
    $('#showLess').click(function () {
        x = (x - 3 < 0) ? 3 : x - 3;
        $('#SeenSeriesList td').not(':lt(' + x + ')').hide();
    });
    $("#lightSlider").lightSlider({
        item: 3,
        slideMargin: 0,
        speed: 400,
        auto: true,
        pauseOnHover: true,
        loop: true,
        pause: 2000,

        responsive: [
            {
                breakpoint: 1070,
                settings: {
                    item: 2,
                    slideMove: 1,
                }
            },
            {
                breakpoint: 800,
                settings: {
                    item: 1,
                }
            }],
    });
});

//Home
function search(searchedVal, table) {
    var value = searchedVal.toLowerCase().trim();
    var tableString = "#" + table + " td";
    $(tableString).show();
    $("#" + table + " .card").filter(function () {
        $(this).toggle($(this).find('.card-title').text().toLowerCase().indexOf(value) > -1)
    });
}

function deleteRow(id) {
    var r = confirm("Are you sure you want to delete this record?");
    if (r == true) {
        window.location.assign("/Home/Delete/" + id);
    }
}
//_SeasonsAndEpisodes
function seenEpisode(id, buttonId) {
    $.ajax({
        url: '/Home/SeenEpisode',
        data: { "id": id },
        type: 'POST',
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            if (result) {
                console.log(buttonId);
                $(buttonId).removeClass("unseen").addClass("seen");

            }
        }
    });
}
//Edit - TODO fix
function AddEpisodes(id) {
    var SeasonNo = document.getElementById("SeasonNo").value;
    var EpisodesNo = document.getElementById("EpisodesNo").value;
    $.ajax({
        url: '/Home/AddEpisodes',
        type: 'POST',
        dataType: 'text',
        data: { 'id': id, 'SeasonNo': SeasonNo, 'EpisodesNo': EpisodesNo },
        success: function (result) {
            if (result) {
                $("#divEpisodes").load(location.href + " #divEpisodes");
                $('#addSeason').modal('hide');
            }
        }
    });
}