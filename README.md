# Meethub

_Este proyecto consiste es una aplicación multiplataforma para la gestión de eventos presenciales como fiestas, reuniones, conciertos… La aplicación permite crear y gestionar tanto los eventos como los asistentes asociados, albergando también funciones como generar entradas, realizar un check-in con codigos QR y más._

## Comenzando ✔

_Estas instrucciones te permitirán obtener una guía para la utilización de esta aplicación._


### Accesibilidad 💻

A esta aplicación podrás acceder desde cualquier dispositivo a traves de esté enlace desde de un navegador web.

```
https://mvs.sytes.net/meethub
```

También puedes utilizar una aplicación android en lugar de acceder a través de un navegador, puedes descargar el archivo .apk a través de este enlace.

```
https://drive.google.com/file/d/1Ptt9V4424ujsV7KQoyz-WaT1P83EqCwU/view?usp=drive_link
```

Puedes acceder al proyecto de Android Studio desde este enlace.

```
http://mvs.sytes.net:15002/tfg-repository/meethub-app
```

## Manual de uso ⚙️

_Este manual estará orientado a explicar la funcionalidad y la usabilidad de la aplicación._

#### Videotutorial

```
https://drive.google.com/file/d/1VTNfgM5GLOVEu3zW2qvrv6KZuUhdQEti/view?usp=drive_link
```

### Login

_Lo primero que encontraremos en la aplicación es una vista que contiene un formulario de login con la opción de también registrarnos._

![Login view](https://github.com/defu13/meethub/blob/master/wwwroot/images/login.PNG)

Podremos crear una cuenta introduciendo los siguientes datos:

* Nombre
* Apellidos
* Correo electrónico
* Contraseña

_Para iniciar sesión se utilizará el correo electrónico._

### Inicio

_Después de iniciar sesión entraremos a la vista de inicio la cual, si existen, nos muestra un listado de eventos que estén en curso o programados y toda su información al respecto. Desde esta vista podremos crear nuevos eventos y gestionarlos._

![Login view](https://github.com/defu13/meethub/blob/master/wwwroot/images/home.PNG)

Para crear un evento debemos pulsar el botón "Nuevo", se desplegará una ventana en la cual debemos de introducir los datos del evento en un formulario.

Haciendo click o pulsando en un evento nos llevara a la vista de **evento**.

### Evento

_En esta vista podremos, mediante una interfaz intuitiva, editar y eliminar el evento en cuestión, así como gestionar la asistencia de tus invitados._

![Login view](https://github.com/defu13/meethub/blob/master/wwwroot/images/evento.PNG)

Desde esta vista es posible copiar el enlace de invitación para compartirlo con los asistentes deseados, los cuales se unirán rellenando un formulario de acceso.

Cuando hayamos compartido nuestro enlace de evento, irán apareciendo los asistentes que se registren en una lista de invitados.

Existen dos tipos de eventos:

* Eventos por **inscripción**: Los asistentes se unirán sin ninguna restricción.
* Eventos por **invitación**: Cuando un asistente se registre, el usuario debe aceptar o rechazar su solicitud de entrada mediante un botón que aparecerá en la lista de invitados

### Estadísticas

_Esta vista ofrece una visión de los eventos ya finalizados con información sobre la asistencia que han logrado._

![Login view](https://github.com/defu13/meethub/blob/master/wwwroot/images/stats.PNG)

### Perfil

_Desde aquí podrás cambiar algunos datos de tu perfil y cerrar la sesión actual._

![Login view](https://github.com/defu13/meethub/blob/master/wwwroot/images/perfil.PNG)

## Autores ✒️

* **Yubal De Fuente Carmona** - creador y desarrollador.
