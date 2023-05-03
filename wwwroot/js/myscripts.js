$(document).ready(() => {

    function menuAdjust() {
        if ($(window).width() >= 625) {
            $('.menu-small').addClass('d-none');
            $('.menu-big').removeClass('d-none');
        } else {
            $('.menu-small').removeClass('d-none');
            $('.menu-big').addClass('d-none');
        }
    }

    menuAdjust();

    $(window).on('resize', () => {
        menuAdjust();
    });

    $('.menu-button').on('click', () => {

        //Muestra los iconos vacios
        $('.icon-fill').addClass('d-none');
        $('.icon-empty').removeClass('d-none');

        // CONTROL DE BOTONES DEL MENU
        if ($(event.target).closest('.menu-button').hasClass('menu-button-user')) {
            $('.menu-button-user svg.icon-fill').removeClass('d-none');
            $('.menu-button-user svg.icon-empty').addClass('d-none');
            console.log('user');
            $('#contenedor-paginas').attr('src', 'profile.html');
        } else if ($(event.target).closest('.menu-button').hasClass('menu-button-barchart')) {
            $('.menu-button-barchart svg.icon-fill').removeClass('d-none');
            $('.menu-button-barchart svg.icon-empty').addClass('d-none');
            console.log('stats');
            $('#contenedor-paginas').attr('src', 'stats.html');
        } else if ($(event.target).closest('.menu-button').hasClass('menu-button-home')) {
            $('.menu-button-home svg.icon-fill').removeClass('d-none');
            $('.menu-button-home svg.icon-empty').addClass('d-none');
            console.log('home');
            $('#contenedor-paginas').attr('src', 'home.html');
        }
    });

    function eliminarTildes(texto) {
        return texto.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    }

    $(".search-bar-input").on("keyup", function () {
        var textoBusqueda = eliminarTildes($(this).val().toLowerCase());

        $('.event-card-container').each(function () {
            var textoElemento = eliminarTildes($(this).find('span').text().toLowerCase());
            // console.log(textoElemento.indexOf(textoBusqueda));
            if (textoElemento.indexOf(textoBusqueda) !== -1) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });


        var containersVisibles = $('.event-card-container:visible').length;
        // console.log(containersVisibles);

        if (containersVisibles === 0) {
            $('.event-card-container').hide();
        }

        if ($('.event-card-container:visible').length === 1) {
            $('.event-card-container:visible').addClass('full-width');
        } else {
            $('.event-card-container:visible').removeClass('full-width');
        }
    });

    $('.search-bar-input').on('input', function () {
        if ($(this).val() === '') {
            // El input está vacío
            $('.event-card-container').show();
            $('.event-card-container').removeClass('full-width');
        }
    });

    $('.options-button').on('click', function () {
        var eventTitle = $(this).closest('.event-card').find('span.event-title').text();
        $('#menu-opciones .offcanvas-title').text(eventTitle);
    });

    
});