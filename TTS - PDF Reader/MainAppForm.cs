using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VersOne.Epub;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace TTS___PDF_Reader
{
    public partial class MainAppForm : Form
    {
        public MainAppForm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "PDF files|*.pdf", ValidateNames = true, Multiselect = false })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(ofd.FileName);
                        StringBuilder sb = new StringBuilder();
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            sb.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        }
                        rtbxText.Text = sb.ToString();
                        reader.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void btnEpub_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "EPUB files|*.epub", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        EpubBook ebook = EpubReader.ReadBook(ofd.FileName);
                        StringBuilder sb = new StringBuilder();
                        foreach (EpubChapter chapter in ebook.Chapters)
                        {
                            sb.Append(MainAppForm.PrintChapter(chapter));
                        }
                        rtbxText.Text = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private static string PrintChapter(EpubChapter chapter)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(chapter.HtmlContent);
            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//text()"))
            {
                sb.AppendLine(node.InnerText.Trim());
            }
            return sb.ToString();

            /*string chapterTitle = chapter.Title;
            string chapterText = sb.ToString();
            Console.WriteLine("------------ ", chapterTitle, "------------ ");
            Console.WriteLine(chapterText);
            Console.WriteLine();
            foreach (EpubChapter subChapter in chapter.SubChapters)
            {
                PrintChapter(subChapter);
            }*/
        }

        SpeechSynthesizer speech = new SpeechSynthesizer();

        private void btnSpeak_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(rtbxText.Text))
                {
                    speech.Dispose();
                    speech = new SpeechSynthesizer();
                    speech.SpeakAsync(rtbxText.Text);
                }
                else
                {
                    MessageBox.Show("Ningun texto ha sido cargado, por favor seleccione uno e intente nuevamente");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (speech != null)
                {
                    if (speech.State == SynthesizerState.Speaking)
                    {
                        speech.Pause();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            try
            {
                if (speech != null)
                {
                    if (speech.State == SynthesizerState.Paused)
                    {
                        speech.Resume();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (speech != null)
                {
                    speech.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
