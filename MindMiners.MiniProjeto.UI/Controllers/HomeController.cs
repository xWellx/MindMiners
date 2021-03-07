using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MindMiners.MiniProjeto.Applications.Srt;
using MindMiners.MiniProjeto.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISrtApplication _srtApplication;
        private readonly IMemoryCache _memoryCache;
        const string CACHENAME = "CacheSRTFiles";
        public HomeController(ILogger<HomeController> logger, ISrtApplication srtApplication, IMemoryCache memoryCache)
        {
            _logger = logger;
            this._srtApplication = srtApplication;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void Cache(string name, object file)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            _memoryCache.Set(name, file, cacheEntryOptions);
        }

        private void AddFileToCache(byte[] file)
        {
            List<(DateTime date, byte[] file)> listFilesCached = GetCacheSrt();
            listFilesCached.Add((DateTime.Now, file));
            Cache(CACHENAME, listFilesCached);
        }

        private List<(DateTime date, byte[] file)> GetCacheSrt()
        {
            if (_memoryCache.TryGetValue(CACHENAME, out object cacheObject))
            {
                return (List<(DateTime, byte[])>)cacheObject;
            }

            return new List<(DateTime date, byte[] file)>();
        }

        public async Task<IActionResult> Sendsub(SendsubModel sendsubModel)
        {
            var files = sendsubModel.Files;

            if (files.Count == 0)
            {
                ViewBag.Error = "Ao menos um arquivo .srt deve ser enviado";
                return View("index");
            }

            _logger.LogInformation($"{files.Count} arquivo(s) enviado(s)");

            if (files.Any(x => !x.FileName.Contains(".srt")))
            {
                ViewBag.Error = "Somente arquivos srt são permitidos";
                return View("index");
            }

            long size = files.Sum(f => f.Length);

            _logger.LogInformation($"tamanho total {size} bytes");

            if (size > 1000 * 10000)
            {
                ViewBag.Error = "Arquivo(s) muito grande para importar";
                return View("index");
            }

            foreach (var formFile in files)
            {
                _logger.LogInformation($"processando arquivo {formFile.Name}");

                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);

                        byte[] fileBytes = memoryStream.ToArray();

                        byte[] bystesFileWithOffset = await _srtApplication
                            .CreateSrtFileWithOffsetAsync(fileBytes, sendsubModel.Offset);

                        AddFileToCache(bystesFileWithOffset);

                        return File(bystesFileWithOffset,
                                    "application/srt",
                                    $"{Guid.NewGuid().ToString("N")}.srt");
                    }
                }
            }

            return View("index");
        }

        public IActionResult History()
        {
            return View(GetCacheSrt());
        }


        [Route("history/{index}")]
        public IActionResult History(int index)
        {
            List<(DateTime date, byte[] file)> listSrt = GetCacheSrt();

            try
            {
                return File(listSrt[index].file,
                   "application/srt",
                   $"{Guid.NewGuid().ToString("N")}.srt");
            }
            catch (ArgumentOutOfRangeException e)
            {
                ViewBag.Error = "Indice não encontrado";
                return View("history");
            }
        }
    }
}
