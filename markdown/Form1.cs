using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace markdown
{
    public partial class Form1 : Form
    {
        public class WordLine {
            public string word;
            public int line;
            public int firstLine;
            public string value {
                get {
                    return string.Format("[{0}](#{1})", word, Math.Max(firstLine - 2, 1));
                }
            }
            public override string ToString() {
                return string.Format("[{0}](#{1})", word, Math.Max(line - 2, 1));
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "文本文件|*.txt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(path, Encoding.UTF8);

                int contextCount = 0;
                List<WordLine> words = new List<WordLine>();
                bool isContext = true;
                for (int i = 0, imax = lines.Length; i < imax; i++) {
                    string line = lines[i];
                    //检查到以下部分为单词部分
                    if (line == "----------") {
                        isContext = false;
                        continue;
                    }
                    
                    lines[i] = line + "<br>" + string.Format("<div id=\"{0}\"></div>", i + 1);
                    if (isContext)
                    {
                        contextCount++;
                    }
                    else {
                        if (lines[i][0] == '>') {
                            WordLine wl = new WordLine();
                            wl.line = i + 1;
                            wl.word = line.Substring(1).Trim();
                            words.Add(wl);
                        }
                    }
                }

                List<WordLine> tmpWords = new List<WordLine>(words);
                for (int i = 0; i < contextCount; i++) {
                    string line = lines[i];

                    for (int j = words.Count - 1; j >= 0; j--) {
                        var wl = words[j];
                        if (line.Contains(wl.word)) {
                            line = line.Replace(wl.word, wl.ToString());
                            wl.firstLine = i + 1;
                            words.RemoveAt(j);
                            lines[wl.line - 1] = lines[wl.line - 1].Replace(wl.word, wl.value);
                        }
                    }
                    lines[i] = line;
                }

                File.WriteAllLines(path, lines, Encoding.UTF8);
                MessageBox.Show("转换成功");
            }
        }
    }
}
