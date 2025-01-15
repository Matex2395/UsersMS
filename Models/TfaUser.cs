using System;
using System.Collections.Generic;

namespace LoginMS.Models;

public partial class TfaUser
{
    public int UsersId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserLastName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public int UserPoints { get; set; }

    public int? RolId { get; set; }

    public int? RolIdaddional { get; set; }

    public string Contrasenia { get; set; } = null!;

    public string? UrlImage { get; set; }

    public virtual TfaRol? Rol { get; set; }

    public virtual TfaRol? RolIdaddionalNavigation { get; set; }
}
