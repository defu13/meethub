﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace meethub.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    [NotMapped]
    public bool MantenerSesion {get; set;}
}
