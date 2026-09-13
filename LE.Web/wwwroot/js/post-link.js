// P1 item 11: mutating actions (cancel/delete/enable/disable/day-close save)
// are POST + [ValidateAntiForgeryToken] on the server. This helper converts
// anchors tagged with the 'post-link' class into antiforgery-protected form
// submissions, so Razor loop rows need only:
//   <a href="/area/controller/action/123" class="post-link [confirm-class]">...</a>
// Optional data-confirm="..." shows a confirm() dialog before submitting.
// The antiforgery token is read from a hidden input generated anywhere on the
// page (each view renders @Html.AntiForgeryToken() once), or refreshed from
// /Antiforgery/Token when absent.
$(function () {
    $(document).on('click', 'a.post-link', function (event) {
        event.preventDefault();
        var link = $(this);

        var confirmMsg = link.data('confirm');
        if (confirmMsg && !confirm(confirmMsg)) {
            return false;
        }

        var submit = function (token) {
            var form = $('<form/>', {
                method: 'post',
                action: link.attr('href')
            });
            if (token) {
                form.append($('<input/>', { type: 'hidden', name: '__RequestVerificationToken', value: token }));
            }
            form.appendTo('body').submit();
        };

        var token = $('input:hidden[name="__RequestVerificationToken"]').first().val();
        if (token) {
            submit(token);
        } else {
            // No token on this page yet — fetch one from the antiforgery endpoint.
            $.ajax({
                url: '/Antiforgery/Token',
                type: 'GET',
                success: function (data) {
                    submit(data ? data.token : null);
                },
                error: function () {
                    // Last resort: submit without token; the server will reject
                    // with 400 if antiforgery is enforced, surfacing the problem.
                    submit(null);
                }
            });
        }
        return false;
    });
});
