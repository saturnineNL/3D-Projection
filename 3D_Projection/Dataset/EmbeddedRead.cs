using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Projection.Dataset
{
    static class EmbeddedRead
    {


        public static string GetJSON()

        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            const string path = "_3D_Projection.Dataset.systems.json";

            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
