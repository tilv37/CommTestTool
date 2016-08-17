using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommTestTool.Common
{
    public class FileOprate
    {
        public static List<byte[]> GeByteses(String fileName)
        {
            List<byte[]> tempList=new List<byte[]>();
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                tempList.Add(CommonFucntion.StringToBytes(line));
            }
            return tempList.Count == 0 ? null : tempList;
        } 
    }
}
