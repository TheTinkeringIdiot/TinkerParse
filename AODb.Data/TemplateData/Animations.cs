
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AODb.Data.TemplateData
{
    public class Animations 
    {

        public int UNKValue1;
        public List<KeyValuePair<int, int>> UNKValues2;


        public Animations() 
        { 
            UNKValues2 = new List<KeyValuePair<int, int>>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            //Placeholder for now until I figure out how this works

            this.UNKValue1 = reader.ReadInt32();
            int itemCount = reader.ReadInt32();
            itemCount = (itemCount / 1009) - 1;
            for(int i = 0; i < itemCount; i++) 
            {
                int value2 = reader.ReadInt32();
                int itemCount2 = reader.ReadInt32();
                itemCount2 = (itemCount2 / 1009) - 1;
                for(int j = 0; j < itemCount2; j++)
                {
                    int subval = reader.ReadInt32();
                    this.UNKValues2.Add(new KeyValuePair<int, int>(value2, subval));
                }
            }
        }
    }
}
