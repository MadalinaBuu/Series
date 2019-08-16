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