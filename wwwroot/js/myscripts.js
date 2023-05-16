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

    // function resetModal() {
    //     // Restablecer los estilos del fondo del modal
    //     $('.modal-backdrop').remove();
    // }
    // resetModal();

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

    $('.options-button').off('click').on('click', function () {
        var eventTitle = $(this).closest('.event-card').find('span.event-title').text();
        $('#menu-opciones .offcanvas-title').text(eventTitle);

        if ($('.offcanvas-backdrop').length > 1) {
            $('.offcanvas-backdrop').not(':first').remove();
        }
    });

    $('.create-event-button').on('click', () => {
        if ($('.modal-backdrop').length > 1) {
            $('.modal-backdrop').not(':first').remove();
        }
    });

    $('.delete-file-button').on('click', () => {
        $('#inputGroupFile').val('');
    });

    $('#inputGroupFile').on('change', function () {
        var file = $(this).prop('files')[0];
        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i; // Extensiones permitidas: jpg, jpeg, png

        if (!allowedExtensions.exec(file.name)) {
            alert('Solo se permiten archivos de imagen (jpg, jpeg, png).');
            $(this).val(''); // Restablecer el valor del input file si no es una imagen válida
            return false;
        }
    });

    $('#aforo-switch').on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#aforo-input').prop('disabled', !isChecked);
        if(!$(this).isChecked){
            $('#aforo-input').val('');
        }
    });

    $('#inscripcion-input').on('change', function() {
        if ($(this).is(':checked')) {
            var textInscripcion = 'Con el tipo inscripción puedes compartir tu enlace de evento con la gente que quieras sin necesidad de confirmación del asistente.';
            $('.text-help').text(textInscripcion);
        }
    });

    $('#invitacion-input').on('change', function() {
        if ($(this).is(':checked')) {
            var textInvitacion = 'Con el tipo invitación las personas que se registren en tu evento deberán esperar a que tu les confirmes o rechaces la asistencia.';
            $('.text-help').text(textInvitacion);
        }
    });
});