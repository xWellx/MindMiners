using MindMiners.MiniProjeto.Domains.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Domains.Srt
{
    public interface ISrtDomain
    {
        Task<SrtBlockEntity> ConvertSrtBlockToEntityAsync(string srtBlockText);
        Task<IReadOnlyList<SrtBlockEntity>> ConvertSrtToListEntityAsync(string srtText);
        IReadOnlyList<SrtBlockEntity> IncludeOffsetOnSrtBlock(IReadOnlyList<SrtBlockEntity> srtBlockEntities, double offset);
        string BuildSrtTextFromSrtBlock(IReadOnlyList<SrtBlockEntity> srtBlockEntitiesWithOffSet);
    }
}
