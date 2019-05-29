using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultas.Data
{
    /// <summary>
    /// Classe custom de user.
    /// </summary>
    public class User : IdentityUser
    {
        public string Nome { get; set; }
    }
}
