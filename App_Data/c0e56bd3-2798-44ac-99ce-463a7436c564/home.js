$(function () {
    $('#content .features').each(function (index, element) {
        var delay = 5;
        var container = $(this);
        var features = $('.feature', container);
        if (features.length <= 1) { return; }
        var index = -1;
        window.setInterval(show, delay * 1000);
        show();

        function show() {
            index++;
            if (index >= features.length) { index = 0; }
            features.hide().eq(index).show();
        }
    });
});