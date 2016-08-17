using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Data.Json;

namespace LifxApp
{
    public class Color
    {
        public double hue { get; set; }
        public double saturation { get; set; }
        public int kelvin { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Windows.UI.Color HSBtoRGB(double hue, double saturation, double brightness)
        {
            int r = 0, g = 0, b = 0;
            if (saturation == 0)
            {
                r = g = b = (int)(brightness * 255.0f + 0.5f);
            }
            else
            {
                float h = ((float)hue - (float)Math.Floor(hue)) * 6.0f;
                float f = h - (float)Math.Floor(h);
                float p = (float)brightness * (1.0f - (float)saturation);
                float q = (float)brightness * (1.0f - (float)saturation * f);
                float t = (float)brightness * (1.0f - ((float)saturation * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(t * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(q * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return Windows.UI.Color.FromArgb(Convert.ToByte(255), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        public string ToRGBString()
        {
            return "rgb:" + R.ToString() + "," + G.ToString() + "," + B.ToString();
        }
    }

    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Location
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Capabilities
    {
        public bool has_color { get; set; }
        public bool has_variable_color_temp { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public string company { get; set; }
        public string identifier { get; set; }
        public Capabilities capabilities { get; set; }
    }

    [DataContract]
    public class LightData
    {
        //Memory stream and serialization objects
        private MemoryStream ms = new MemoryStream();
        private DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LightData));

        public string ToJSON()
        {
            ser.WriteObject(ms, this); //Serialize this class and write it to the memory stream
            ms.Position = 0; //Reset the memory stream pointer
            StreamReader sr = new StreamReader(ms); 
            return sr.ReadToEnd(); //Read the memory stream and return the resulting value
        }

        public LightData ToObject()
        {
            ms.Position = 0; //Reset the memory stream pointer
            LightData ldObj = (LightData)ser.ReadObject(ms); //Deserialize the memory stream, return an object and convert that object to a LightData object
            return ldObj;
        }
        
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string uuid { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public bool connected { get; set; }

        [DataMember]
        public string power { get; set; }

        [DataMember]
        public Color color { get; set; }

        [DataMember]
        public double brightness { get; set; }

        [DataMember]
        public Group group { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public string last_seen { get; set; }

        [DataMember]
        public double seconds_since_seen { get; set; }

        [DataMember]
        public Product product { get; set; }
    }

    [CollectionDataContract]
    public class LightCollection : ICollection<LightData>
    {
        public List<LightData> Lights = new List<LightData>();

        private MemoryStream ms = new MemoryStream();
        private DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LightCollection));

        public void Add(LightData ld)
        {
            Lights.Add(ld);
        }

        public void Clear()
        {
            Lights = null;
        }

        public bool Contains(LightData ld)
        {
            if (Lights.Contains(ld))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(LightData[] ldArray, int index)
        {
            Lights.CopyTo(ldArray, index);
        }

        public bool Remove(LightData ld)
        {
            return Lights.Remove(ld);
        }

        public int Count { get; set; }

        public bool IsReadOnly { get; set; }

        public IEnumerator<LightData> GetEnumerator()
        {
            return Lights.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string ToJSON()
        {
            ser.WriteObject(ms, this);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public LightCollection FromJSON(string json)
        {
            JsonArray j = JsonArray.Parse(json);
            JsonObject cObj = JsonObject.Parse(j[0].ToString());
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            ms.Position = 0;
            LightCollection Lights = (LightCollection)ser.ReadObject(ms);
            ms.Dispose();
            return Lights;
        }
    }
}
