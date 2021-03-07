using MindMiners.MiniProjeto.Domains.Entity;
using MindMiners.MiniProjeto.Domains.Srt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Applications.Srt
{
    class SrtApplication : ISrtApplication
    {
        private readonly ISrtDomain _srtDomain;

        public SrtApplication(ISrtDomain srtDomain)
        {
            this._srtDomain = srtDomain;
        }

        public async Task<byte[]> CreateSrtFileWithOffsetAsync(byte[] srtFile, double offset)
        {
            string srtTextFromFile = Encoding.UTF8.GetString(srtFile);

            IReadOnlyList<SrtBlockEntity> srtBlockEntities = await _srtDomain.ConvertSrtToListEntityAsync(srtTextFromFile);

            IReadOnlyList<SrtBlockEntity> srtBlockEntitiesWithOffSet = _srtDomain.IncludeOffsetOnSrtBlock(srtBlockEntities, offset);

            string srtTextFromBuild = _srtDomain.BuildSrtTextFromSrtBlock(srtBlockEntitiesWithOffSet);

            return Encoding.UTF8.GetBytes(srtTextFromBuild);
        }
    }
}
