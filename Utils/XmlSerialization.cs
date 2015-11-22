using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Utils
{
    public class XmlSerialization
    {
        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        public static string Obj2XmlStr(object obj, string nameSpace)
        {
            if (obj == null) return string.Empty;
            var sr = new XmlSerializer(obj.GetType());
            var sb = new StringBuilder();
            var w = new StringWriter(sb, CultureInfo.InvariantCulture);
            sr.Serialize(
                w,
                obj,
                new XmlSerializerNamespaces(
                    new[]
                        {
                            new XmlQualifiedName("", nameSpace)
                        }
                    ));
            return sb.ToString();
        }

        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        public static string Obj2XmlStr(object obj)
        {
            if (obj == null) return string.Empty;
            var sr = new XmlSerializer(obj.GetType());
            var sb = new StringBuilder();
            var w = new StringWriter(sb, CultureInfo.InvariantCulture);
            sr.Serialize(
                w,
                obj,
                new XmlSerializerNamespaces(new[] {new XmlQualifiedName(string.Empty)}));
            return sb.ToString();
        }

        /// <summary>
        /// Десериализует строку XML в объект заданного типа
        /// </summary>
        /// <param name="xml">XML-строка</param>
        /// <param name="type">желаемый тип объекта</param>
        /// <returns></returns>
        public static T XmlStr2Obj<T>(string xml)
        {
            if (xml == null) return default(T);
            if (xml == string.Empty) return (T) Activator.CreateInstance(typeof (T));

            var reader = new StringReader(xml);
            var sr = new XmlSerializer(typeof (T)); //SerializerCache.GetSerializer(type);
            return (T) sr.Deserialize(reader);
        }

        public static void SerializeToXML(object paramObject, string paramNameSpace, string paramFilename)
        {
            TextWriter textWriter = new StreamWriter(paramFilename);
            SerializeToXML(paramObject, paramNameSpace, textWriter);
            textWriter.Close();
        }

        public static void SerializeToXML(object paramObject, string paramFilename)
        {
            TextWriter textWriter = new StreamWriter(paramFilename);
            SerializeToXML(paramObject, textWriter);
            textWriter.Close();
        }

        public static void SerializeToXML(object paramObject, string paramNameSpace, TextWriter paramTextWriter)
        {
            if (paramObject == null) return;
            var serializer = new XmlSerializer(paramObject.GetType());
            serializer.Serialize(paramTextWriter, paramObject, new XmlSerializerNamespaces(new[]
                {
                    new XmlQualifiedName("", paramNameSpace)
                }
                                                                   ));
        }

        public static void SerializeToXML(object paramObject, TextWriter paramTextWriter)
        {
            if (paramObject == null) return;
            var serializer = new XmlSerializer(paramObject.GetType());
            serializer.Serialize(paramTextWriter, paramObject, new XmlSerializerNamespaces(new[]
                {
                    new XmlQualifiedName(string.Empty)
                }
                                                                   ));
        }

        public static BaseType DeserializeFromXML<BaseType>(string paramFilename)
        {
            TextReader textReader = new StreamReader(paramFilename);
            var objects = DeserializeFromXML<BaseType>(textReader);
            textReader.Close();
            return objects;
        }

        public static BaseType DeserializeFromXML<BaseType>(TextReader paramTextReader)
        {
            var deserializer = new XmlSerializer(typeof (BaseType));
            var baseObject = (BaseType) deserializer.Deserialize(paramTextReader);
            return baseObject;
        }
        
        /// <summary>
        /// Сериализует объект в строку XML (DataContractSerializer)
        /// </summary>
        public static string WcfObj2XmlStr(object obj)
        {
            if (obj == null) return string.Empty;
            var sr = new DataContractSerializer(obj.GetType());
            using (var swr = new StringWriter())
            {
                using (var twr = new XmlTextWriter(swr))
                {
                    sr.WriteObject(twr, obj);
                    return swr.ToString();
                }
            }
        }

        /// <summary>
        /// Десериализует строку XML в объект заданного типа (DataContractSerializer)
        /// </summary>
        public static T WcfXmlStr2Obj<T>(string xml)
        {
            if (xml == null) return default(T);
            if (xml == string.Empty) return (T) Activator.CreateInstance(typeof (T));

            var sr = new DataContractSerializer(typeof (T));
            using (var srd = new StringReader(xml))
            {
                using (var trd = new XmlTextReader(srd))
                {
                    return (T) sr.ReadObject(trd);
                }
            }
        }
    }
}