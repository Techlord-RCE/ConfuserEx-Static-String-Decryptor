using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using System.IO;
using dnlib.DotNet.Writer;

namespace ConfuserEx_Static_String_Decryptor
{
    public partial class Form1 : Form
    {
        private string DirectoryName;
        private int amount;

        public Form1()
        {
            InitializeComponent();
        }

        public static string path;
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null || textBox1.Text == "")
                MessageBox.Show("Please Select File");
            else
            {
                path = textBox1.Text;
                ModuleDefMD LoadedFile = ModuleDefMD.Load(textBox1.Text);
                if (checkBox1.Checked)
                {
                    amount = StringDecryptor.InvokeDecryption(LoadedFile);
                }
                else
                {
                    amount = StringDecryptor.Run(LoadedFile);
                }
                
                Excess_Nop_Remover.NopRemover(LoadedFile);
                label1.Text = "Strings Decrypted : " + amount;
                string filename = DirectoryName +"\\"+ Path.GetFileNameWithoutExtension(textBox1.Text) + "-Decrypted" + Path.GetExtension(textBox1.Text);
                var opts = new ModuleWriterOptions(LoadedFile);
                opts.Logger = DummyLogger.NoThrowInstance;
                var writerOptions = new NativeModuleWriterOptions(LoadedFile);
                writerOptions.MetaDataOptions.Flags |= MetaDataFlags.PreserveAll;
                writerOptions.Logger = DummyLogger.NoThrowInstance;


                if (LoadedFile.IsILOnly)
                {
                    LoadedFile.Write(filename, opts);
                }
                else
                {
                    LoadedFile.NativeWrite(filename, writerOptions);
                }
                

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            this.textBox1.Text = "";
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Browse for target assembly",
                InitialDirectory = @"c:\"
            };
            if (DirectoryName != "")
            {
                dialog.InitialDirectory = DirectoryName;
            }
            dialog.Filter = "All files (*.exe,*.dll)|*.exe;*.dll";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                textBox1.Text = fileName;
                int startIndex = fileName.LastIndexOf(@"\");
                if (startIndex != -1)
                {
                    DirectoryName = fileName.Remove(startIndex, fileName.Length - startIndex);
                }
                if (DirectoryName.Length == 2)
                {
                    DirectoryName = DirectoryName + @"\";
                }
            }

        }
    }
}
