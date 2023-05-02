using System;
using System.Collections.Generic;

namespace mvc_pruebas.Models;

public partial class EventsAssistant
{
    public int IdEventAssistant { get; set; }

    public int? IdEvent { get; set; }

    public int? IdAssistant { get; set; }

    public virtual Assistant? IdAssistantNavigation { get; set; }

    public virtual Event? IdEventNavigation { get; set; }
}
