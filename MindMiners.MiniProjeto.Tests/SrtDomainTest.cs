using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using MindMiners.MiniProjeto.Domains;
using MindMiners.MiniProjeto.Domains.Entity;
using MindMiners.MiniProjeto.Domains.Exceptions;
using MindMiners.MiniProjeto.Domains.Srt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Tests
{
    public class SrtDomainTest
    {
        ISrtDomain _srtDomain;
        Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            DomainStartup.Startup(serviceCollection);
            var serviceBuild = serviceCollection.BuildServiceProvider();
            _srtDomain = serviceBuild.GetService<ISrtDomain>();

            _fixture = new Fixture();
        }

        [Test]
        [TestCase(@"168a
00:20:41,150 --> 00:20:45,109
- How did he do that?
- Made him an offer he couldn't refuse.")]
        public void ConvertSrtBlockTextWithErrorIndexInvalid(string srtBlockText)
        {
            var exception = Assert.ThrowsAsync<CustomException>(() => _srtDomain.ConvertSrtBlockToEntityAsync(srtBlockText));

            Assert.AreEqual("Falha ao converter o indice da SRT", exception.Message);
        }


        [Test]
        [TestCase(@"168
00:20:41,150 -> 00:20:45,109
- How did he do that?
- Made him an offer he couldn't refuse.")]
        public void ConvertSrtBlockTextWithErrorSeparatorNotExist(string srtBlockText)
        {
            var exception = Assert.ThrowsAsync<CustomException>(() => _srtDomain.ConvertSrtBlockToEntityAsync(srtBlockText));

            Assert.AreEqual("Separador de datas não existe", exception.Message);
        }

        [Test]
        [TestCase(@"168
00:20:41,150 --> 
- How did he do that?
- Made him an offer he couldn't refuse.")]
        public void ConvertSrtBlockTextWithErrorDateNotFound(string srtBlockText)
        {
            var exception = Assert.ThrowsAsync<CustomException>(() => _srtDomain.ConvertSrtBlockToEntityAsync(srtBlockText));

            Assert.AreEqual("Não foram encontradas datas suficientes", exception.Message);
        }

        [Test]
        [TestCase(@"168
00:20:41,150 --> 00:20:45,109
- How did he do that?
- Made him an offer he couldn't refuse.")]
        public async Task ConvertSrtBlockTextWithValid(string srtBlockText)
        {
            var result = await _srtDomain.ConvertSrtBlockToEntityAsync(srtBlockText);

            Assert.NotZero(result.Index);
            Assert.IsNotEmpty(result.Text);
            Assert.Greater(result.Start, new DateTime());
            Assert.Greater(result.End, new DateTime());
        }

        [Test]
        [TestCase(@"1
00:00:03,400 --> 00:00:06,177
In this lesson, we're going to
be talking about finance. And

2
00:00:06,177 --> 00:00:10,009
one of the most important aspects
of finance is interest.

3
00:00:10,009 --> 00:00:13,655
When I go to a bank or some
other lending institution

4
00:00:13,655 --> 00:00:17,720
to borrow money, the bank is happy
to give me that money. But then I'm

5
00:00:17,900 --> 00:00:21,480
going to be paying the bank for the
privilege of using their money. And that

6
00:00:21,660 --> 00:00:26,440
amount of money that I pay the bank is
called interest. Likewise, if I put money

7
00:00:26,620 --> 00:00:31,220
in a savings account or I purchase a
certificate of deposit, the bank just")]
        public async Task ConvertSrtTextWithValid(string srtBlockText)
        {
            var result = await _srtDomain.ConvertSrtToListEntityAsync(srtBlockText);

            Assert.AreEqual(result.Count, 7);
        }



        [Test]
        public void IncludeOffsetOnSrtBlockValid()
        {
            Fixture fixture = new Fixture();

            List<SrtBlockEntity> srtBlockEntities = fixture.CreateMany<SrtBlockEntity>().ToList();
            double offSet = fixture.Create<double>();

            var result = _srtDomain.IncludeOffsetOnSrtBlock(srtBlockEntities, offSet);

            var tupleJoin = srtBlockEntities
                   .Join(result,
                   srtBlock => srtBlock.Index,
                   srtBlockOffset => srtBlockOffset.Index,
                   (srtBlock, srtBlockOffset) => (srtBlock, srtBlockOffset));

            foreach (var (srtBlock, srtBlockOffset) in tupleJoin)
            {
                Assert.AreEqual(srtBlock.Index, srtBlockOffset.Index);
                Assert.AreEqual(_srtDomain.AddOffsetToSrtTime(srtBlock.Start, offSet).TimeOfDay, srtBlockOffset.Start.TimeOfDay);
                Assert.AreEqual(_srtDomain.AddOffsetToSrtTime(srtBlock.End, offSet).TimeOfDay, srtBlockOffset.End.TimeOfDay);
                Assert.AreEqual(srtBlock.Text.Count, srtBlockOffset.Text.Count);
            }
        }

        [Test]
        public void AddOffsetToSrtTimeValid()
        {
            DateTime time = _fixture.Create<DateTime>();
            double offSet = _fixture.Create<double>();

            DateTime timeOffset = _srtDomain.AddOffsetToSrtTime(time, offSet);

            Assert.GreaterOrEqual(time, timeOffset);
        }

        [Test]
        public void AddOffsetToSrtTimeWithNegativeValue()
        {
            DateTime time = _fixture.Create<DateTime>();
            double offSet = -50000000;

            DateTime timeOffset = _srtDomain.AddOffsetToSrtTime(time, offSet);

            Assert.AreEqual(timeOffset.Date, timeOffset);
        }
    }
}