using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class PessoaJuridica
{
    public int Idlocatario { get; set; }

    public string Cnpj { get; set; } = null!;

    public virtual Locatario IdlocatarioNavigation { get; set; } = null!;
}
