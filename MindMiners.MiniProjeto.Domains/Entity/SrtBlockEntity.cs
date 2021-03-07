using System;
using System.Collections.Generic;
using System.Text;

namespace MindMiners.MiniProjeto.Domains.Entity
{
    public class SrtBlockEntity
    {
        public int Index { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> Text { get; set; }
    }
}
