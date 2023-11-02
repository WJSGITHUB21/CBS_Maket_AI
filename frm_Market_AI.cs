using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBS_KursLib;
using CBS_DlgLib;
using CBS_Komponenten;
using System.Runtime.InteropServices;
using System.IO;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using DlgMarketLib;



namespace CBS_MarketAI
{
    public partial class frm_Market_AI : CBSForm
    {

        Cmkt_adressen2List adrList;

        Cmkt_adressen2List katList;
        Cconfigur config;

        String aFilter = "";

        Cn_aktivitaetList selecAktivi;

        public frm_Market_AI()
        {
            InitializeComponent();
        }

        public frm_Market_AI(Cpwd aUser, CBSForm aMainForm)
        {
            User = aUser;
            _mainForm = aMainForm;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (User.Id < 0) // der Anwender hat keine gültige Nutzer-Kennung eingegeben
            {
                Close();
                return;
            }

            adrList = new Cmkt_adressen2List();
            bsMarketAI.DataSource = adrList.Daten;
            bnMarketAI.BindingSource = bsMarketAI;
            bnMarketAI.DataListObject = adrList;  // Notwendig wenn Button Refresh

            katList = new Cmkt_adressen2List();
            bsKategorie.DataSource = katList.Daten;



            selecAktivi = new Cn_aktivitaetList(" anzeigenjanein > 0 ");
            bsSelecAktivi.DataSource = selecAktivi.Daten;

            tabControlAlphabet.SelectedTabPage = tabAlle;
            xtraABCKatFilter.SelectedTabPage = tabAlle;

            listBoxControl1.Items.Add("");
            Filtere_Kategorien();
        }

        private void tabControlAlphabet_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            Filtere_Datenmenge();
        }

        private void btnFiltere_Click(object sender, EventArgs e)
        {
            Filtere_Datenmenge();
        }

        private void Filtere_Datenmenge()
        {
            
            String hand = "";


            aFilter = "";
            if (tabControlAlphabet.SelectedTabPage.Name == "tabAlle")
            {
                aFilter = "";
            }
            else
            {
                aFilter += hand + " mkt_adressen2.Entidade LIKE '" + tabControlAlphabet.SelectedTabPage.Text + "%'  ";
                hand = " and ";
            }

            if (listBoxControl1 != null)
            {
                if (listBoxControl1.SelectedItem != null)
                {
                    if (listBoxControl1.SelectedItem.ToString() != "")
                    {
                        aFilter += hand + " mkt_adressen2.Aktivitaet LIKE '%" + listBoxControl1.SelectedItem.ToString() + "%'  ";
                        hand = " and ";
                    }
                }
            }

            if (suchbegriff1.Text != "")
            {
                aFilter += hand + " mkt_adressen2.Entidade LIKE '%" + suchbegriff1.Text + "%'  ";
                hand = " and ";
            }

            if (suchbegriff2.Text != "")
            {
                aFilter += hand + " mkt_adressen2.Entidade LIKE '%" + suchbegriff2.Text + "%'  ";
                hand = " and ";
            }

            if (tbSuchOrt.Text != "")
            {
                aFilter += hand + " mkt_adressen2.Ort LIKE '%" + tbSuchOrt.Text + "%' OR mkt_adressen2.Stadt LIKE '%" + tbSuchOrt.Text + "%' OR" +
                                               " mkt_adressen2.Concelho LIKE '%" + tbSuchOrt.Text + "%' OR mkt_adressen2.Distrikt LIKE '%" + tbSuchOrt.Text + "%'";
                hand = " and ";
            }

            if ((SuchPLZ1.Text != "") && (SuchPLZ1.Text == ""))
            {
                aFilter += hand + " mkt_adressen2.PLZ LIKE '%" + SuchPLZ1.Text + "%'  ";
                hand = " and ";
            }
            else
            if ((SuchPLZ1.Text != "") && (SuchPLZ1.Text != ""))
            {
                aFilter +=  hand + " (mkt_adressen2.PLZ >= '" + SuchPLZ1.Text + "' AND " +
                                               "  mkt_adressen2.PLZ < '" + SuchPLZ2.Text + "')     ";
             
                hand = " and ";
            }

            if (ceURL.Checked)
            {
                aFilter += hand + " (mkt_adressen2.URL != '') " ;

                hand = " and ";
            }

            if (ceEmail.Checked)
            {
                aFilter += hand + " (mkt_adressen2.email != '') ";

                hand = " and ";
            }

            if (suchEmail.Text != "")
            {
                aFilter += hand + " mkt_adressen2.email LIKE '%" + suchEmail.Text + "%'  ";
                hand = " and ";
            }

            if (lb_cae.Text != "")
            {
                aFilter += hand + " mkt_adressen2.cae LIKE '" + lb_cae.Text + "%'  ";
                hand = " and ";
            }

            adrList.Filter = aFilter;
            bsMarketAI.DataSource = adrList.Daten;
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            listBoxControl1.Items.Clear();
            Filtere_Kategorien();
        }

        private void Filtere_Kategorien()
        {
            this.Cursor = Cursors.WaitCursor;

            String neu = "";
            String alt = "";

            Cmkt_adressen2List al = new Cmkt_adressen2List("  mkt_adressen2.Aktivitaet LIKE '" + xtraABCKatFilter.SelectedTabPage.Text + "%' ORDER BY aktivitaet");
            foreach (Cmkt_adressen2 akat in al.Daten)
            {
            //    neu = GetFirstWordOfString(akat.Aktivitaet);
                neu = akat.Aktivitaet;
                if (neu != alt)
                    listBoxControl1.Items.Add(akat.Aktivitaet);
                alt = neu;
            }

            this.Cursor = Cursors.Default;

        }



        private string GetFirstWordOfString(String token)
        {
            int index = token.IndexOf(' ');
            if (index == -1)
                return token;
            else
                return token.Substring(0, index);
        }


        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listBoxControl1.SelectedIndex != 0)
                Filtere_Datenmenge();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            Int32 _markiert = 0;
            foreach (Cmkt_adressen2 ma in adrList.Daten)
            {
                if (ma.Tag > 0)
                    _markiert += 1;
            }

            if (_markiert == 0)
            {
                MessageBox.Show("Falta marcar um registo.");
                return;
            }

            if (listBoxAkti.ItemCount == 0)
            {
                MessageBox.Show("Falta defenir pelo menos uma atividade!");
                return;
            }

            if (_markiert == 1)
            {
                if (MessageBox.Show("Quer transferir os dados do registo nmarcado ?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    Cmkt_adressen2 ma = (Cmkt_adressen2)bsMarketAI.Current;
                    if (ma == null)
                        return;

                    String message = pruefung(ma);

                    if (message != "")
                    {
                        if (MessageBox.Show(message + Environment.NewLine + Environment.NewLine + " Quer transferir na mesma ?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            MessageBox.Show(uebertrage_satz(ma));
                        }
                    }
                }
            }

            if (_markiert > 1)
            {
                if (MessageBox.Show("Quer transferir os  " + _markiert.ToString() + " registos nmarcados\n\rtodos ccom a(s) mesma(s) atividade(s) ?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (Cmkt_adressen2 ma in adrList.Daten)
                    {
                        if (ma.Tag > 0)
                        {
                            String message = pruefung(ma);
                            if (message != "")
                            {
                                if (MessageBox.Show(message + Environment.NewLine + Environment.NewLine + " Quer transferir na mesma ?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    MessageBox.Show(uebertrage_satz(ma));
                            }
                            else
                                MessageBox.Show(uebertrage_satz(ma));
                        }
                    }
                }
            }

        }

        private String uebertrage_satz(Cmkt_adressen2 ma)
        {
            String erg = "";


                Cmkt_firmenpool fi = new Cmkt_firmenpool();

                //Die Transaktion wird gestartet mit der ersten Klasse
                fi.StartTransaction();


                fi.Anrede = 3;
                fi.Aufnahmetag = DateTime.Today;
                fi.Bildname = "";
                fi.Email = ma.Email;
                fi.Fax = ma.Fax;
                fi.Geaendert = DateTime.Now;
                try { fi.Gruendung = Convert.ToDateTime("ma.Gruendung"); }
                catch { fi.Gruendung = Convert.ToDateTime("1900.01.01"); }
                fi.I1 = 0;
                fi.I2 = 0;
                fi.I3 = 0;
                fi.Id = 0;
                fi.Iddistrikt = 0;
                fi.Idjurform = 0;
                fi.Idkategorie = 0;
                fi.Idland = 0;
                fi.Kurzname = "";
                fi.Mobile1 = "";
                fi.Mobile2 = "";
                fi.Name1 = ma.Entidade;
                fi.Name2 = txtComercial.Text;
                fi.Name3 = "";
                fi.Nib = "";
                fi.Ort = ma.Ort;
                fi.Ortsteil =
                fi.Plz = ma.Plz;
                fi.Pwd = "";
                fi.S1 = "";
                fi.S2 = "";
                fi.S3 = "";
                fi.Stadt = ma.Stadt;
                fi.Steuernr = ma.Nif;
                fi.Strasse = ma.Strasse;
                fi.Telefon = ma.Telefon;
                fi.S1 = ma.Gestor;
                fi.Url = ma.Url;

            if (fi.Insert() == DBErrConstants.OK)
            {
                //WILH 202004 umgestellt v. mkt_firmen auf mkt_firmenpool ( hat keine Mitgl.)
                Cmkt_mitglied m;

                config = new Cconfigur();
                config.LoadKeyGenerate("MKT_FuncGeral", "5");
                Int32 _fgeral = Convert.ToInt32(config.Wert);

                m = new Cmkt_mitglied();
                // m wird mit in die Transaktion aufgenommen - andere Handhabe - ORIGAL BertramBorh
                m.Transaction = fi.Transaction;

                m.Geaendert = DateTime.Now;
                m.Idfirma = fi.Id;
                m.Email = fi.Email;
                m.Fax = fi.Fax;
                m.Kurzname = "";
                m.Mobile1 = "";
                m.Mobile2 = "";
                m.Name1 = fi.Name1;
                m.Pwd = "";
                m.Telefon = fi.Telefon;
                m.Idfunktion = _fgeral;
                m.Url = fi.Url;

                if (m.Insert() == DBErrConstants.OK)
                {

                    Cmkt_v_firmenpoolaktiv mktnp;
                    foreach (String item in listBoxAkti.Items)
                    {
                        String[] res = item.Split(';');
                        mktnp = new Cmkt_v_firmenpoolaktiv();
                        mktnp.Transaction = fi.Transaction;
                        mktnp.Idaktivitaet = Convert.ToInt32(res[1]);
                        mktnp.Idfirma = fi.Id;
                        mktnp.Insert();
                    }

                    (fi.Transaction as SqlTransaction).Commit();

                    adrList.Filter = adrList.Filter;
                    bsMarketAI.DataSource = adrList.Daten;
                    erg = gTT("Die Firma und deren Gestor wurden erfolgreich uebertragen", ProgSprache);
                }
                else
                    erg = gTT("Die Firma wurde nicht transferiert:\n\r", ProgSprache);

            }
            return (erg);

        }



            private String pruefung(Cmkt_adressen2 adr2)
        {
            String erg = "";
            String _filter;


            string eMailTeiStr;
            try
            {
                eMailTeiStr = adr2.Email.Substring(1, 7);
            }
            catch (Exception)
            {
                eMailTeiStr = "";
                erg += " Falta o email!" + Environment.NewLine;
            }

            if (eMailTeiStr != "")
            {

                _filter = "eMail like '%" + eMailTeiStr + "%'";
                Cmkt_firmenpoolList mktfl = new Cmkt_firmenpoolList(_filter);

                if (mktfl.Daten.Count > 0)
                {
                    foreach (Cmkt_firmenpool adf in mktfl.Daten)
                    {
                        erg += "eMail existente parecido: " + Environment.NewLine + adf.Id.ToString() + " " + adf.Name2 + " -  " + adf.Email + Environment.NewLine + Environment.NewLine;
                    }
                }
            }

            string NomeTeilStr;
            try
            {
                NomeTeilStr = adr2.Entidade.Substring(1, 7);
            }
            catch (Exception)
            {
                NomeTeilStr = "";
                erg += " Falta o nome da Entidade!" + Environment.NewLine;
            }


            if (NomeTeilStr != "")
            {
                _filter = "name1 like '%" + NomeTeilStr + "%' OR name2 like '%" + NomeTeilStr + "%'";
                Cmkt_firmenpoolList mktfl2 = new Cmkt_firmenpoolList(_filter);

                if (mktfl2.Daten.Count > 0)
                {
                    foreach (Cmkt_firmenpool adf2 in mktfl2.Daten)
                    {
                        erg += "Nome existente parecido: " + Environment.NewLine + adf2.Id.ToString() + " " + adf2.Name1 + " " + adf2.Name2 + Environment.NewLine;
                    }
                }
            }

            return(erg);
        }


        private void repositoryItemCheckEdit1_CheckStateChanged(object sender, EventArgs e)
        {
            Cmkt_adressen2 c = (Cmkt_adressen2)bsMarketAI.Current;
            CheckEdit ce = (DevExpress.XtraEditors.CheckEdit)sender;
            if (c != null)
            {
                if (ce.Checked)
                    c.Tag = 1;
                else
                    c.Tag = 0;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Quer apagar para sempre as entidades marcadas?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (Cmkt_adressen2 d in adrList.Daten)
                {
                    if (d.Tag == 1)
                    {
                        if (d.Delete() != DBErrConstants.OK)
                        {
                            MessageBox.Show("Wegen eines Fehlers beim Loeschen der Daten abgebrochen", "Achtung");
                            return;
                        }
                    }
                }


                adrList.Filter = aFilter;
                bsMarketAI.DataSource = adrList.Daten;
            }
        }

        private void btn_CloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_minimizeForm_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnSprachRef_setzen_Click(object sender, EventArgs e)
        {
            Cmkt_adressen2 c = (Cmkt_adressen2)bsMarketAI.Current;
            if (c == null)
                return;


            Cn_aktivitaet sr = (Cn_aktivitaet)bsSelecAktivi.Current;
            if (sr != null)  // wenn Datensatz vorhanden ist
            {
                listBoxAkti.Items.Add(sr.Text + "; " + sr.Id.ToString() + "; " + c.Id.ToString());
            }
        }



        private void simpleButton2_Click(object sender, EventArgs e)
        {
            listBoxAkti.Items.Clear();
        }

        private void btnURL_Click(object sender, EventArgs e)
        {

            if (hyperLinkEdit1.Visible)
                hyperLinkEdit1.Visible = false;
            else
                hyperLinkEdit1.Visible = true;
        }
    }



}
