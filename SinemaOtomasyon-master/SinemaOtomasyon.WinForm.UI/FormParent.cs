﻿using Ninject;
using SinemaOtomasyon.Repository.Repositories.Abstracts;
using SinemaOtomasyon.WinForm.UI.AdminIslemleri;
using SinemaOtomasyon.WinForm.UI.Ninject;
using SinemaOtomasyon.WinForm.UI.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinemaOtomasyon.WinForm.UI
{
    public partial class FormParent : Form
    {
        private IPersonelRepository _personelRepo;
        private IFilmRepository _filmRepo;
        private ISalonRepository _salonRepo;

        public FormParent()
        {
            var container = NinjectDependencyContainer.RegisterDependency(new StandardKernel());
            _personelRepo = container.Get<IPersonelRepository>();
            _filmRepo = container.Get<IFilmRepository>();
            _salonRepo = container.Get<ISalonRepository>();
            InitializeComponent();
        }

        private void FormParent_Load(object sender, EventArgs e)
        {
            #region GirişYapanKullanıcıTürüKontrolü
            int role = _personelRepo.GetList().Where(x => x.Login.Username == FormLogin.Username).Select(x => x.Login.Role.RoleID).FirstOrDefault();
            if (role != 1)
            {
                btnFilmIslemleri.Enabled = false;
                btnGosterimIslemleri.Enabled = false;
                btnPersonelslemleri.Enabled = false;
            }
            #endregion

            BosalacakSalonKontrol();
            lblUsername.Text = FormLogin.Username;
        }

        private void BosalacakSalonKontrol()
        {
            var salonkontrol = _filmRepo.GetList().Where(x => x.Vizyonda == true && x.VizyonCksTarih < DateTime.Now.Date).Select(x => new { salonid = x.Salon.SalonID }).ToList();
            foreach (var salloon in salonkontrol)
            {
                _salonRepo.GetById(salloon.salonid).DoluMu = false;
                if (_salonRepo.Save() > 0)
                {
                    MessageBox.Show(salloon.salonid + " numaralı salon boşalmıştır.");
                }
            }
        }

        private void btnFilmIslemleri_Click(object sender, EventArgs e)
        {
            FormFilmSeansSalonSec frmFilm = new FormFilmSeansSalonSec();
            frmFilm.ShowDialog();
        }

        private void btnFilmIslemleri_Click_1(object sender, EventArgs e)
        {
            FormFilmIslemleri frmFilmIslemleri = new FormFilmIslemleri();
            frmFilmIslemleri.ShowDialog();
        }

        private void btnPersonelslemleri_Click(object sender, EventArgs e)
        {
            FormPersonelIslemleri frmPersonel = new FormPersonelIslemleri();
            frmPersonel.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult onayla = new DialogResult();
            onayla = MessageBox.Show("Onaylıyor musunuz ?", "Uyarı", MessageBoxButtons.YesNo);

            if (onayla == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void FormParent_Leave(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormParent_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormLogin frm = new FormLogin();
            frm.Show();
        }

        private void btnFatura_Click(object sender, EventArgs e)
        {

            FormFatura frmFatura = new FormFatura();
            frmFatura.ShowDialog();
        }
    }
}
