using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.WebApp
{
    public static class HardCoding//hard coding wrapper
    {
        public static class Logic
        {
            public const string Loader_Section = "LoaderData";

            public const string Raw_Assembly = "RawLogic_AssemblyName";
            public const string Raw_Type = "RawLogic_TypeName";

            public const string Analyzer_Assembly = "Analyzer_AssemblyName";
            public const string Analyzer_Type = "Analyzer_TypeName";

            public const string Reporter_Assembly = "Reporter_AssemblyName";
            public const string Reporter_Type = "Reporter_TypeName";

            public const string Pigtail_Assembly = "PigtailLogic_AssemblyName";
            public const string Pigtail_Type = "PigtailLogic_TypeName";
        }//


        public static class View
        {
            static View()
            {
                SizeValues = new int[] { 15, 50, 100 };     //사이즈 설정값
                
                PagerSizes = new Dictionary<int, string>();
                for (int i = 0; i < SizeValues.Length; i++)
                    PagerSizes.Add(SizeValues[i], (SizeValues[i] != 0) ? SizeValues[i].ToString() : "All");
            }

            //MVC6 Grid 페이지 사이즈
            public static Dictionary<int, string> PagerSizes;
            static int[] SizeValues;

        }


    }//class

}
