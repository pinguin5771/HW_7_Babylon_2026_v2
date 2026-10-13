using Galactic.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp_Babylon_2026_v2
{
    public partial class Form2 : Form
    {
        private readonly string path = string.Empty;
        public Form2()
        {
            InitializeComponent();
            //Galactic.Core.IAncientScript

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Artifacts");
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Galactic");
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            Scan();
        }

        private void buttonLoadScripts_Click(object sender, EventArgs e)
        {
            LoadAncientScripts();
        }

        private void Scan()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] artifactsArray = Directory.GetFiles(path);

            foreach(var itemPath in artifactsArray)
            {
                string data = LibraryScanner.ScanLibrary(itemPath);
                stringBuilder.Append(data);
                stringBuilder.Append(Environment.NewLine);
                Debug.WriteLine(data);
            }

            textBoxScanResult.Text = stringBuilder.ToString();
        }

        private void LoadAncientScripts()
        {
            List<IAncientScript> scripts = new List<IAncientScript>();
            string[] libraries = Directory.GetFiles(path, "*.dll");

            foreach(var libraryItem in libraries)
            {
                Assembly assembly = Assembly.LoadFrom(libraryItem);
                foreach(Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    object instance = Activator.CreateInstance(type);

                    if(instance is IAncientScript knownScript)
                    {
                        scripts.Add(knownScript);
                    }
                    else if(type.FullName.Contains("KlingonData"))
                    {
                        scripts.Add(new KlingonAdapter(instance));
                    }
                    else
                    {
                        Debug.WriteLine("Does not supports IAncientScript: " + type.FullName);
                    }
                    // else if(type.FullName)
                    // 1. instance is IAncientScript -> Add to scripts List
                    // 2. instance is not implements IAncientScript -> Add adapter and then Add as IAncientScript
                    // 3. we can do nothing with this object       
                }
            }

            IAncientScript ancientScript = scripts[1];
            string origin = ancientScript.GetOrigin();
            string text = ancientScript.Translate("Text to translate");
            Image image = ancientScript.GetEmblem();

            foreach(IAncientScript script in scripts)
            {
                try
                {
                    listBoxScripts.Items.Add($"Origin: {script.GetOrigin()}");
                    listBoxScripts.Items.Add($"Translation: {script.Translate("HELLO!")}");

                    Image emblem = script.GetEmblem();

                    listBoxScripts.Items.Add($"Emblem available: " +
                        $"{(emblem != null ? "Yes" : "No")}");

                    listBoxScripts.Items.Add("--------------------------------");
                }
                catch (Exception ex)
                {
                    listBoxScripts.Items.Add($"Error testing script: {ex.Message}");
                }
            }
            if (scripts.Count == 0)
            {
                listBoxScripts.Items.Add("No scripts loaded.");
            }
        }
    }


    public class MaintenanceDroidAdapter : IAncientScript
    {
        //   [METHODS]:
        //-> Void GetSystemLogs()
        // Parameters:
        //    - String query
        //    - out String status

        private readonly object droid;
        private readonly MethodInfo method;


        public MaintenanceDroidAdapter(object droid)
        {
            this.droid = droid;
            method = droid.GetType().GetMethod("GetSystemLogs");
        }

        public Image GetEmblem() => null;

        public string GetOrigin() => "Unknown Droid Unit";

        public string Translate(string rawData)
        {
            if(method != null)
            {
                object[] parameters = new object[] { rawData, null };
                method.Invoke(droid, parameters);
                string translationResult = parameters[1]?.ToString() ?? "Error";

                return translationResult;
            }
            return string.Empty;
        }
    }

    public class Transformer
    {
        public int getVoltage()
        {
            return 230;
        }
    }

    public class Ua_Transformer : IAdapter
    {
        public int GetVoltage()
        {
            return 230;
        }
    }

    public class De_Transformer : IAdapter
    {
        public int GetVoltage()
        {
            return 230;
        }
    }

    public class Jp_Transformer
    {
        public int GetLowVoltage()
        {
            return 110;
        }
    }

    // 1. схема підключення
    // 2. коробка яка містить сторонній обєкт і забезпечить
    //          підключення через нашу схему підключення

    public interface IAdapter
    {
        int GetVoltage();
    }

    public class JapanTransformerAdapter : IAdapter
    {
        private Jp_Transformer jp_Transformer;
        public JapanTransformerAdapter(Jp_Transformer jp_Transformer)
        {
            this.jp_Transformer = jp_Transformer;
        }

        public int GetVoltage()
        {
            return MultiplyVoltage(jp_Transformer.GetLowVoltage());
        }

        private int MultiplyVoltage(int voltage)
        {
            return voltage * 2;
        }
    }
    public class EnergyLine
    {
        public void Connect()
        {
            List<IAdapter> transformers = new List<IAdapter>();

            var ua_Transformer = new Ua_Transformer();
            var jp_Transformer = new Jp_Transformer();
            var japanTransformerAdapter = new JapanTransformerAdapter(jp_Transformer);

            transformers.Add(ua_Transformer);
            transformers.Add(japanTransformerAdapter);

            foreach(var item in transformers)
            {
                int voltage = item.GetVoltage();
            }
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public class KlingonAdapter : IAncientScript
    {
        private readonly object klingon;
        private readonly MethodInfo decodeMethod;
        private readonly MethodInfo insigniaMethod;

        public KlingonAdapter(object klingon)
        {
            this.klingon = klingon;
            Type type = klingon.GetType();
            decodeMethod = type.GetMethod("DecodeWarriorCry");
            insigniaMethod = type.GetMethod("GetWarriorInsignia");
        }

        public string GetOrigin()
        {
            return "Klingon Warrior";
        }

        public string Translate(string rawData)
        {
            return decodeMethod.Invoke(klingon, 
                new object[] {rawData})?.ToString() ?? string.Empty;
        }

        public Image GetEmblem()
        {
            return insigniaMethod.Invoke(klingon, null) as Image;
        }
    }

}
