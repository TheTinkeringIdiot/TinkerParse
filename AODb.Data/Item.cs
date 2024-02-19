
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AODb.Common;
using AODb.Common.Attributes;
using AODb.Data.TemplateData;

namespace AODb.Data
{
    public class Item
    {
        [StreamData(0)]
        public uint DBType { get; set; }

        [StreamData(1)]
        public uint AOID { get; set; }

        [StreamData(2)]
        public uint Version { get; set; }

        [StreamData(3)]
        public uint UNK1 { get; set; }


        public List<StatValue> StatValues { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public AtkDefData AttackDefenseData { get; set; }
        public AnimationMesh AnimationMesh { get; set; }

        public ActionData ActionData { get; set; }

        public List<SpellData> SpellData { get; set; }

        public Sounds Sounds { get; set; }

        public ShopHashData ShopHashData { get; set; }

        public Item() 
        { 
            this.StatValues = new List<StatValue>();
            this.SpellData = new List<SpellData>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {



            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(StreamDataAttribute)))
                .OrderBy(p => p.GetCustomAttributes().OfType<StreamDataAttribute>().First().Index)
                .ToArray();
            
            foreach(PropertyInfo property in properties)
            {
                StreamDataAttribute attrib = (StreamDataAttribute)Attribute.GetCustomAttribute(property, typeof(StreamDataAttribute));

                if(attrib != null)
                {
                    property.SetValue(this, reader.ReadUInt32());
                }
            }

            bool foundName = false;
            StatValue sv = new StatValue();
            while(!foundName)
            {
                sv.PopulateFromStream(reader);

                // Apparently a double usage of this stat key
                if(sv.Stat == Stat.Psychic) { foundName = true; }
                else 
                { 
                    this.StatValues.Add(sv); 
                    sv = new StatValue(); 
                }
            }

            int nameLen = reader.ReadInt16();
            int descLen = reader.ReadInt16();

            Name = Encoding.Default.GetString(reader.ReadBytes(nameLen));
            Description = Encoding.Default.GetString(reader.ReadBytes(descLen));

            // if(this.AOID == 266576)
            if(this.AOID == 259783)
            {

            }
            

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int key = reader.ReadInt32();
                // Console.WriteLine($"Found Key: {key}");

                switch(key)
                {
                    case (int)ItemTemplateDataType.Invalid:
                        break;

                    case (int)ItemTemplateDataType.Spells:
                        SpellData spellData = new SpellData();
                        spellData.PopulateFromStream(reader);
                        this.SpellData.Add(spellData);
                        break;

                    case (int)ItemTemplateDataType.AtkDef:
                        AtkDefData atkdef = new AtkDefData();
                        atkdef.PopulateFromStream(reader);
                        this.AttackDefenseData = atkdef;
                        break;

                    case (int)ItemTemplateDataType.AnimationMesh:
                        AnimationMesh aniMesh = new AnimationMesh();
                        aniMesh.PopulateFromStream(reader);
                        this.AnimationMesh = aniMesh;
                        break;

                    case (int)ItemTemplateDataType.Animations:
                        // Console.WriteLine("ITEM HAS ANIMATIONS");
                        Animations anims = new Animations();
                        anims.PopulateFromStream(reader);
                        break;

                    case (int)ItemTemplateDataType.Stats:
                        // Console.WriteLine("ITEM HAS STAT TYPE");
                        break;

                    case (int)ItemTemplateDataType.Sounds:
                        // Console.WriteLine("ITEM HAS SOUNDS");
                        Sounds sounds = new Sounds();
                        sounds.PopulateFromStream(reader);
                        this.Sounds = sounds;
                        break;

                    case (int)ItemTemplateDataType.Actions:
                        ActionData actionData = new ActionData();
                        actionData.PopulateFromStream(reader);
                        this.ActionData = actionData;
                        break;

                    case (int)ItemTemplateDataType.ShopHash:
                        ShopHashData shd = new ShopHashData();
                        shd.PopulateFromStream(reader);
                        this.ShopHashData = shd;
                        break;

                }


            }
        }
    }
}
