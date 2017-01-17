$(function () {
    $('#locations').each(function (index, element) {
        var container = $(this);
        var mapCanvas = $('#map-canvas');
        var locations = $('.location', container);
        var noResults = $('.locations > p', container).hide();
        var markers = [];

        var mapOptions = {
            zoom: 8
        };
        var map = new google.maps.Map(mapCanvas[0], mapOptions);
        var geocoder = new google.maps.Geocoder();
        var centerMarker = new google.maps.Marker({ map: map, visible: false, icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png' });
        var infowindow = new google.maps.InfoWindow({ content: '' });

        locations.each(function (index, element) {
            var el = $(this);
            var lat = el.attr('data-lat');
            var lon = el.attr('data-lon');
            var title = el.find('span.name').text();

            var marker = new google.maps.Marker({
                position: new google.maps.LatLng(lat, lon),
                icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',
                animation: google.maps.Animation.DROP,
                map: map,
                title: title,
                html: el.html(),
                id: el.attr('id'),
                el: el
            });

            google.maps.event.addListener(marker, 'click', function () {
                infowindow.setContent(marker.html);
                infowindow.open(map, marker);
            });

            markers.push(marker);
        });

        FitMarkers();

        function FitMarkers() {
            var bound = new google.maps.LatLngBounds();
            var visibleCount = 0;
            $.each(markers, function (i, marker) {
                if (marker.visible) {
                    bound.extend(marker.getPosition());
                    visibleCount++;
                }
            });
            console.log('visibleCount: ' + visibleCount);
            if (visibleCount > 0) { map.fitBounds(bound); }
            noResults.toggle(visibleCount == 0);
        }




        var zipInput = $('input[type="text"]', container);
        var distanceInput = $('select', container);
        $('.view-all', container).on('click', function (e) {
            e.preventDefault();
            $.each(markers, function (index, element) {
                markers[index].setVisible(true);
                markers[index].el.toggle(true);
            });
            centerMarker.setVisible(false);
            FitMarkers();
        });

        $('input[type="button"]', container).on('click', function (e) {
            e.preventDefault();
            var zip = zipInput.val();
            var distance = distanceInput.val();

            geocoder.geocode({ 'address': zip }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    console.log(results[0]);
                    var center = results[0].geometry.location;
                    map.setCenter(center);
                    centerMarker.setPosition(center);
                    centerMarker.setVisible(true);

                    $.each(markers, function (index, element) {
                        var miles = (google.maps.geometry.spherical.computeDistanceBetween(center, markers[index].position) * 0.000621371192);
                        var show = (miles < distance);
                        markers[index].setVisible(show);
                        markers[index].el.toggle(show);
                    });

                    FitMarkers();
                }
                else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        });


    });

});