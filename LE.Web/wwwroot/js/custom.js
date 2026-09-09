$(function () {
    $('.delete').on('click', function (event) {
        if (!confirm("Are you sure to delete?")) {
            event.preventDefault();
        }
    });
})

let noRows = $("#number_of_rows").val()
$("#number_of_rows").change(function () {
    noRows = $("#number_of_rows").val();
})

let noPage = $(".pagination").find(".current").html();

if (noPage != null || noPage != undefined) {
    let pageno = noPage.match(/\d+/)[0];

    let sn = ((pageno - 1) * noRows) + 1;

    $(".table > tbody > tr").each(function (index) {
        $(this).find("td:first").html(sn);
        sn++;
    })
}


