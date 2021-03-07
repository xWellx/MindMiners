using MindMiners.MiniProjeto.Domains.Entity;
using MindMiners.MiniProjeto.Domains.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Domains.Srt
{
    class SrtDomain : ISrtDomain
    {
        public async Task<SrtBlockEntity> ConvertSrtBlockToEntityAsync(string srtBlockText)
        {
            SrtBlockEntity srtItemEntity = new SrtBlockEntity();

            using (StringReader sr = new StringReader(srtBlockText))
            {
                int index = 0;
                while ((await sr.ReadLineAsync()) is string line && line != null)
                {
                    switch (index)
                    {
                        case 0:
                            if (!Int32.TryParse(line, out int indexLegenda))
                                throw new CustomException("Falha ao converter o indice da SRT");

                            srtItemEntity.Index = indexLegenda;

                            break;
                        case 1:
                            string separate = "-->";

                            if (!line.Contains(separate))
                                throw new CustomException("Separador de datas não existe");

                            var arrDates = line.Replace(",", ".").Split(separate, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            if (arrDates.Length < 2)
                                throw new CustomException("Não foram encontradas datas suficientes");

                            if (DateTime.TryParse(arrDates[0], out DateTime start))
                            {
                                srtItemEntity.Start = start;
                            }
                            else
                                throw new CustomException("Data inicial não é válida");

                            if (DateTime.TryParse(arrDates[1], out DateTime end))
                            {
                                srtItemEntity.End = end;
                            }
                            else
                                throw new CustomException("Data final não é válida");
                            break;
                        default:
                            (srtItemEntity.Text ??= new List<string>()).Add(line);
                            break;
                    }

                    index++;
                }
            }

            return srtItemEntity;
        }

        private Task<SrtBlockEntity> ProcessStringBuilder(ref StringBuilder stringBuilder)
        {
            string text = stringBuilder.ToString();
            stringBuilder.Clear();
            return ConvertSrtBlockToEntityAsync(text);
        }

        public async Task<IReadOnlyList<SrtBlockEntity>> ConvertSrtToListEntityAsync(string srtText)
        {
            List<SrtBlockEntity> listSrtItemEntity = new List<SrtBlockEntity>();

            using (StringReader sr = new StringReader(srtText))
            {
                var tasks = new List<Task<SrtBlockEntity>>();

                StringBuilder stringBuilder = new StringBuilder();

                while ((await sr.ReadLineAsync()) is string line)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        stringBuilder.AppendLine(line);
                    else
                    {
                        tasks.Add(ProcessStringBuilder(ref stringBuilder));
                    }
                }

                if (stringBuilder.Length > 0)
                {
                    tasks.Add(ProcessStringBuilder(ref stringBuilder));
                }

                var resultSrtItemEntity = await Task.WhenAll(tasks);

                listSrtItemEntity.AddRange(resultSrtItemEntity);
            }

            return listSrtItemEntity;
        }

        public DateTime AddOffsetToSrtTime(DateTime dateTime, double offset)
        {
            var newTime = dateTime.AddMilliseconds(offset);
            if (newTime.Date != dateTime.Date)
            {
                return newTime.Date;
            }


            return dateTime;
        }

        public IReadOnlyList<SrtBlockEntity> IncludeOffsetOnSrtBlock(IReadOnlyList<SrtBlockEntity> srtBlockEntities, double offset)
        {
            return srtBlockEntities.Select(srtblock => new SrtBlockEntity()
            {
                Index = srtblock.Index,
                End = AddOffsetToSrtTime(srtblock.End, offset),
                Start = AddOffsetToSrtTime(srtblock.Start, offset),
                Text = srtblock.Text
            }).ToList();
        }

        public string BuildSrtTextFromSrtBlock(IReadOnlyList<SrtBlockEntity> srtBlockEntitiesWithOffSet)
        {
            string dateSrtToString(DateTime dateTime)
            {
                return dateTime.ToString("HH:mm:ss,fff");
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var srtBlock in srtBlockEntitiesWithOffSet.OrderBy(x => x.Index))
            {
                stringBuilder.AppendLine(srtBlock.Index.ToString());
                stringBuilder.AppendLine($"{dateSrtToString(srtBlock.Start)} --> {dateSrtToString(srtBlock.End)}");

                srtBlock.Text.ForEach(text =>
                {
                    stringBuilder.AppendLine(text);
                });

                stringBuilder.AppendLine(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }
}
