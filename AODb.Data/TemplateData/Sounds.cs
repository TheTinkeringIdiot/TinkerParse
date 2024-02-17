
using System.Collections.Generic;
using System.IO;

namespace AODb.Data.TemplateData
{
    public class Sounds 
    {

        public int UNKValue1;
        public List<KeyValuePair<int, int>> UNKValues2;


        public Sounds() 
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
