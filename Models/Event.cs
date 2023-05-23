﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace meethub.Models;

public partial class Event
{
    public int IdEvent { get; set; }

    public int IdUser { get; set; }

    [Column(TypeName = "LONGBLOB")]
    public byte[]? Image { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int? Aforo { get; set; }

    public string Direccion { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Tipo { get; set; } = null!;

    public string Enlace { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
