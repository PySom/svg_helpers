using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RemoveWhiteSpace
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            else
            {
                string[] intention = args[0].Split('|');
                if(intention[0] == "remove")
                {
                    if(args.Length == 2 && char.TryParse(args[1], out char separator))
                    {
                        RemoveWhiteSpace(intention[1], separator);
                    }
                    else
                    {
                        RemoveWhiteSpace(intention[1]);
                    }
                }
                else if(intention[0] == "replace")
                {
                    ReplaceSvgEmbeddedStyleWithInlineStyle(intention[1]);
                }
                else
                {
                    Console.WriteLine("We do not understand your command");
                }
            }
        }

        static void RemoveWhiteSpace(string directory, char separator = '_')
        {
            try
            {
                var fileNames = Directory.GetFiles(directory);
                foreach (var fileName in fileNames)
                {
                    string currentName = fileName.Split("\\")[^1];
                    if (currentName.Contains(' '))
                    {
                        string newName = currentName.Replace(' ', separator);
                        File.Move(Path.Combine(directory, currentName), Path.Combine(directory, newName));
                    }
                }
                Console.WriteLine("Done!");
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occured when trying to read from the file. {0}", ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("We do not have permission to read this file");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"{directory} is not a valid argument");
            }
            
        }

        static void ReplaceSvgEmbeddedStyleWithInlineStyle(string directory)
        {
            try
            {
                var fileNames = Directory.GetFiles(directory);

                foreach (var fileName in fileNames)
                {
                    if (fileName.EndsWith("svg"))
                    {
                        XDocument document = XDocument.Load(fileName);
                        XElement svg_Element = document.Root;
                        XElement style = svg_Element.Descendants("{http://www.w3.org/2000/svg}style").Select(path => path).FirstOrDefault();
                        //check if a style tag was declared
                        if (style is object)
                        {
                            //get the style defined as a string eg - .a{top:0;bottom:1}.f{color: black;}
                            var styleDeclaration = style.FirstNode.ToString();
                            //start of style declaration
                            int start = 0;
                            //start of attribute styling
                            var indexOfStyleDeclaration = styleDeclaration.IndexOf('{');
                            //get styles as a list of declarations ef - [".a{top:0;bottom:1}", ".f{color: black;}"]
                            var individualStyleAttributes = new List<string>();
                            for (int currentDeclarationIndex = indexOfStyleDeclaration;
                                currentDeclarationIndex != -1;
                                currentDeclarationIndex = styleDeclaration.IndexOf('{'))
                            {
                                int end = styleDeclaration.IndexOf('}');
                                if (end != -1)
                                {
                                    string name = "";
                                    if (end + 1 == styleDeclaration.Length)
                                    {
                                        name = styleDeclaration.Substring(start);
                                    }
                                    else
                                    {
                                        name = styleDeclaration.Substring(start, end + 1);
                                    }
                                    individualStyleAttributes.Add(name);
                                    styleDeclaration = styleDeclaration.Substring(end + 1);
                                }
                                else
                                {
                                    break;
                                }

                            }
                            //apply style to element with style attribute
                            foreach (var item in individualStyleAttributes)
                            {
                                if (item.Length > 0)
                                {
                                    int begin = item.IndexOf('{') + 1;
                                    var first = item.Substring(1, begin - 2);
                                    int length = item.Length - begin;
                                    var newStyle = item.Substring(begin, length - 1);
                                    var allElems = svg_Element.Descendants().Where(x => x.HasAttributes && x.Attributes().Any(c => c.Value == first)).ToList();
                                    foreach (var elem in allElems)
                                    {
                                        if (!elem.Attributes().Any(c => c.Name == "style"))
                                        {
                                            elem.SetAttributeValue("style", newStyle);

                                        }

                                    }
                                }
                            }
                            style.Remove();
                        }
                        using var stream = XmlWriter.Create(File.OpenWrite(fileName));
                        document.Save(stream);
                    }
                }
                Console.WriteLine("Done!");
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occured when trying to read from the file. {0}", ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("We do not have permission to read this file");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"{directory} is not a valid argument");
            }
            
        }
    }
}
