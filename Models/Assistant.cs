using System;
using System.Collections.Generic;

namespace mvc_pruebas.Models;

public partial class Assistant
{
    public int IdAssistant { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public bool Aprobado { get; set; }

    public bool Confirmado { get; set; }

    public virtual ICollection<EventsAssistant> EventsAssistants { get; set; } = new List<EventsAssistant>();
}
