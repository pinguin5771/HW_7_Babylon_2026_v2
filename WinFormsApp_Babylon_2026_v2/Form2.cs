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

            IAncientScript ancientScript1 = scripts[0];
            string origin = ancientScript1.GetOrigin();
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
            method = droid.GetType().GetMethod();
        }

        public Image GetEmblem() => null;

        public string GetOrigin() => "Unknown Droid Unit";

        public string Translate(string rawData)
        {
            throw new NotImplementedException();
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

}
