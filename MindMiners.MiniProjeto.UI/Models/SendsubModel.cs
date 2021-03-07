using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.UI.Models
{
    public class SendsubModel
    {
        public List<IFormFile> Files { get; set; }
        public int Offset{ get; set; }
    }
}
