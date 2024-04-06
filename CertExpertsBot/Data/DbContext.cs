using CertExpertsBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertExpertsBot.Data
{
    public class DbContext
    {
        #region TechRegs

        private static readonly TechReg DCCU_317_18_06_2010 = CreateTechReg_DCCU_317_18_06_2010();

        #endregion

        public ICollection<TNVEDCode> TNVEDCodes { get; }

        public DbContext()
        {
            TNVEDCodes = CreateTNVEDCodesSet();
        }

        private ICollection<TNVEDCode> CreateTNVEDCodesSet()
        {
            var tNVEDCodes = new HashSet<TNVEDCode>();
            AddTNVEDCodes_Chapter01(tNVEDCodes);

            return tNVEDCodes;
        }

        #region Add Chapters

            #region Add Chapter 01

        private void AddTNVEDCodes_Chapter01(ICollection<TNVEDCode> tNVEDCodes)
        {
            AddTNVEDCodes_Group0101(tNVEDCodes);
            AddTNVEDCodes_Group0102(tNVEDCodes);
            AddTNVEDCodes_Group0103(tNVEDCodes);
            AddTNVEDCodes_Group0104(tNVEDCodes);
            AddTNVEDCodes_Group0105(tNVEDCodes);
        }

        private void AddTNVEDCodes_Group0101(ICollection<TNVEDCode> tNVEDCodes)
        {
            tNVEDCodes.Add(CreateTNVEDCode_0101210000());
            tNVEDCodes.Add(CreateTNVEDCode_0101291000());
            tNVEDCodes.Add(CreateTNVEDCode_0101299000());
            tNVEDCodes.Add(CreateTNVEDCode_0101300000());
            tNVEDCodes.Add(CreateTNVEDCode_0101900000());
        }

        private void AddTNVEDCodes_Group0102(ICollection<TNVEDCode> tNVEDCodes)
        {
            tNVEDCodes.Add(CreateTNVEDCode_0102211000());
            tNVEDCodes.Add(CreateTNVEDCode_0102213000());
            tNVEDCodes.Add(CreateTNVEDCode_0102219000());
            tNVEDCodes.Add(CreateTNVEDCode_0102290500());
            tNVEDCodes.Add(CreateTNVEDCode_0102291000());
            tNVEDCodes.Add(CreateTNVEDCode_0102292100());
            tNVEDCodes.Add(CreateTNVEDCode_0102292900());
            tNVEDCodes.Add(CreateTNVEDCode_0102294100());
            tNVEDCodes.Add(CreateTNVEDCode_0102294900());
            tNVEDCodes.Add(CreateTNVEDCode_0102295100());
            tNVEDCodes.Add(CreateTNVEDCode_0102295900());
            tNVEDCodes.Add(CreateTNVEDCode_0102296100());
            tNVEDCodes.Add(CreateTNVEDCode_0102296900());
            tNVEDCodes.Add(CreateTNVEDCode_0102299100());
            tNVEDCodes.Add(CreateTNVEDCode_0102299900());
            tNVEDCodes.Add(CreateTNVEDCode_0102310000());
            tNVEDCodes.Add(CreateTNVEDCode_0102391000());
            tNVEDCodes.Add(CreateTNVEDCode_0102399000());
            tNVEDCodes.Add(CreateTNVEDCode_0102902000());
            tNVEDCodes.Add(CreateTNVEDCode_0102909100());
            tNVEDCodes.Add(CreateTNVEDCode_0102909900());
        }

        private void AddTNVEDCodes_Group0103(ICollection<TNVEDCode> tNVEDCodes)
        {
            tNVEDCodes.Add(CreateTNVEDCode_0103100000());
            tNVEDCodes.Add(CreateTNVEDCode_0103911000());
            tNVEDCodes.Add(CreateTNVEDCode_0103919000());
            tNVEDCodes.Add(CreateTNVEDCode_0103921100());
            tNVEDCodes.Add(CreateTNVEDCode_0103921900());
            tNVEDCodes.Add(CreateTNVEDCode_0103929000());
        }

        private void AddTNVEDCodes_Group0104(ICollection<TNVEDCode> tNVEDCodes)
        {
            tNVEDCodes.Add(CreateTNVEDCode_0104101000());
            tNVEDCodes.Add(CreateTNVEDCode_0104103000());
            tNVEDCodes.Add(CreateTNVEDCode_0104108000());
            tNVEDCodes.Add(CreateTNVEDCode_0104201000());
            tNVEDCodes.Add(CreateTNVEDCode_0104209000());
        }

        private void AddTNVEDCodes_Group0105(ICollection<TNVEDCode> tNVEDCodes)
        {
            tNVEDCodes.Add(CreateTNVEDCode_0105111100());
            tNVEDCodes.Add(CreateTNVEDCode_0105111900());
            tNVEDCodes.Add(CreateTNVEDCode_0105119100());
            tNVEDCodes.Add(CreateTNVEDCode_0105119900());
            tNVEDCodes.Add(CreateTNVEDCode_0105120000());
            tNVEDCodes.Add(CreateTNVEDCode_0105130000());
            tNVEDCodes.Add(CreateTNVEDCode_0105140000());
            tNVEDCodes.Add(CreateTNVEDCode_0105150000());
            tNVEDCodes.Add(CreateTNVEDCode_0105940000());
            tNVEDCodes.Add(CreateTNVEDCode_0105991000());
            tNVEDCodes.Add(CreateTNVEDCode_0105992000());
            tNVEDCodes.Add(CreateTNVEDCode_0105993000());
            tNVEDCodes.Add(CreateTNVEDCode_0105995000());
        }

        #endregion

        #endregion

        #region Создание кодов ТН ВЭД

            #region 0101

        private TNVEDCode CreateTNVEDCode_0101210000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0101210000",
                Name = "Лошади чистопородные племенные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0101291000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0101291000",
                Name = "Лошади прочие убойные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0101299000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0101299000",
                Name = "Лошади прочие"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0101300000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0101300000",
                Name = "Ослы"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0101900000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0101900000",
                Name = "Прочие мулы и лошаки живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        #endregion

            #region 0102

        private TNVEDCode CreateTNVEDCode_0102211000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102211000",
                Name = "Нетели (самки крупного рогатого скота до первого отела) живые чистопородные племенные, домашний крупный рогатый скот"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102213000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102213000",
                Name = "Коровы живые чистопородные племенные, домашний крупный рогатый скот"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102219000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102219000",
                Name = "Прочий крупный рогатый скот живой домашний, чистопородные племенные животные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102290500()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102290500",
                Name = "Прочий крупный рогатый скот живой, домашний, подрода bibos или подрода poephagus"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102291000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102291000",
                Name = "Прочий крупный рогатый скот живой, домашний, массой не более 80 кг"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102292100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102292100",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 80 кг, но не более 160 кг, убойный"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102292900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102292900",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 80 кг, но не более 160 кг, прочий"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102294100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102294100",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 160 кг, но не более 300 кг, убойный"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102294900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102294900",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 160 кг, но не более 300 кг, прочий"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102295100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102295100",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг: нетели (самки крупного рогатого скота до первого отела) убойные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102295900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102295900",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг: нетели (самки крупного рогатого скота до первого отела) прочие"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102296100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102296100",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг: коровы убойные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102296900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102296900",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг: коровы прочие"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102299100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102299100",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг, убойный"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102299900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102299900",
                Name = "Прочий крупный рогатый скот живой, домашний, массой более 300 кг"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102310000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102310000",
                Name = "Буйволы чистопородные племенные, крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102391000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102391000",
                Name = "Прочие буйволы, домашние виды, крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102399000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102399000",
                Name = "Прочие буйволы, крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102902000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102902000",
                Name = "Прочие чистопородные племенные животные, крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102909100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102909100",
                Name = "Прочие домашние виды, крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0102909900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0102909900",
                Name = "Прочий крупный рогатый скот живой"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        #endregion

            #region 0103

        private TNVEDCode CreateTNVEDCode_0103100000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103100000",
                Name = "Свиньи живые чистопородные племенные животные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0103911000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103911000",
                Name = "Прочие свиньи живые массой менее 50 кг, домашние виды"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0103919000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103919000",
                Name = "Прочие свиньи живые массой менее 50 кг"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0103921100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103921100",
                Name = "Домашние свиноматки живые массой не менее 160 кг, имевшие по крайней мере один опорос"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0103921900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103921900",
                Name = "Прочие домашние виды живых свиней, имеющие массу 50 кг или более"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0103929000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0103929000",
                Name = "Прочие свиньи живые массой 50 кг или более"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        #endregion

            #region 0104

        private TNVEDCode CreateTNVEDCode_0104101000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0104101000",
                Name = "Овцы живые, чистопородные племенные животные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0104103000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0104103000",
                Name = "Ягнята (до одного года), живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0104108000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0104108000",
                Name = "Прочие овцы живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0104201000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0104201000",
                Name = "Козы живые, чистопородные племенные животные"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0104209000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0104209000",
                Name = "Прочие козы живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        #endregion

            #region 0105

        private TNVEDCode CreateTNVEDCode_0105111100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105111100",
                Name = "Цыплята прародительских и материнских линий племенного разведения линии несушек кур домашних вида gallus domesticus массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105111900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105111900",
                Name = "Прочие цыплята прародительских и материнских линий племенного разведения кур домашних вида gallus domesticus, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105119100()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105119100",
                Name = "Прочие домашние куры вида gallus domesticus линии несушек, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105119900()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105119900",
                Name = "Прочие домашние куры вида gallus domesticus, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105120000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105120000",
                Name = "Индейки домашние живые, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105130000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105130000",
                Name = "Утки домашние живые, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105140000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105140000",
                Name = "Гуси домашние живые, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105150000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105150000",
                Name = "Цесарки домашние живые, массой не более 185 г"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105940000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105940000",
                Name = "Прочие куры домашние (gallus domesticus) живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105991000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105991000",
                Name = "Прочие утки живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105992000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105992000",
                Name = "Прочие гуси живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105993000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105993000",
                Name = "Прочие индейки живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        private TNVEDCode CreateTNVEDCode_0105995000()
        {
            var tNVEDCode = new TNVEDCode()
            {
                Code = "0105995000",
                Name = "Прочие цесарки живые"
            };

            tNVEDCode.TechRegs.Add(DCCU_317_18_06_2010);

            return tNVEDCode;
        }

        #endregion

        #endregion

        #region Создание документов тех. регулирования

        private static TechReg CreateTechReg_DCCU_317_18_06_2010()
        {
            var techReg = new TechReg()
            {
                Name = "Решение Комиссии Таможенного Союза №317 от 18.06.2010",
                Description = "Ветеринарный сертификат"
            };

            return techReg;
        }

        #endregion
    }
}
