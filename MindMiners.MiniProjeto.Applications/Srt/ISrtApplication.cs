using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Applications.Srt
{
    public interface ISrtApplication
    {
        Task<byte[]> CreateSrtFileWithOffsetAsync(byte[] srtFile, double offset);
    }
}
