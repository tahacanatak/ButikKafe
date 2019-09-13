﻿using ButikKafe.Data;
using ButikKafe.Properties;
using Newtonsoft.Json;
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

namespace ButikKafe
{
    public partial class Form1 : Form
    {
        int masaAdet = 20;

        KafeVeri db;
        public Form1()
        {
            VerileriOku();
            InitializeComponent();
            MasalariOlustur();
        }

        private void OrnekVeriEkle()
        {
            if (db.Urunler.Count == 0)
            {
                db.Urunler.Add(new Urun { UrunAd = "Ayran", BirimFiyat = 3.50m });
                db.Urunler.Add(new Urun { UrunAd = "Çay", BirimFiyat = 4.50m });
            }
        }

        private void MasalariOlustur()
        {
            #region Resim Listesinin Tanımlanması

            ImageList resimListesi = new ImageList();
            resimListesi.Images.Add("bos", Resources.masabos);
            resimListesi.Images.Add("dolu", Resources.masadolu);
            resimListesi.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = resimListesi;

            #endregion

            #region Masaların Oluşturulması
            int masaNo;
            ListViewItem lvi;
            for (int i = 0; i < masaAdet; i++)
            {
                masaNo = i + 1;
                lvi = new ListViewItem("Masa " + masaNo);
                lvwMasalar.Items.Add(lvi);

                lvi.ImageKey = db.MasaDoluMu(masaNo) ? "dolu" : "bos";
                lvi.Tag = masaNo;

            }
            #endregion
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem tiklanan = lvwMasalar.SelectedItems[0];
            int masaNo = (int)tiklanan.Tag;

            Siparis siparis = db.SiparisBul(masaNo);

            //siparis eger bulunamadıysa oluştur ve ekle
            if (siparis == null)
            {
                siparis = new Siparis(masaNo);
                db.AktifSiparisler.Add(siparis);
                tiklanan.ImageKey = "dolu";
            }

            // ve ilgili masanın siparis formunu aç
            SiparisForm siparisForm = new SiparisForm(db,siparis);
            siparisForm.ShowDialog();


            if (siparis.Durum != SiparisDurum.Aktif)
            {
                tiklanan.ImageKey = "bos";
            }
        }

        private void VerileriYaz()
        {
            string json = JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
            }
            OrnekVeriEkle();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriYaz();

        }
    }
}
