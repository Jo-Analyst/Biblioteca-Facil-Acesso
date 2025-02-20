﻿ 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controle_de_livros
{
    public partial class FrmLivroDidatico : Form
    {
        public FrmLivroDidatico()
        {
            InitializeComponent();
            txt_Registro.Focus();
        }

        Livro_Didatico didatico = new Livro_Didatico();
        string stringConn = Security.Dry("9UUEoK5YaRarR0A3RhJbiLUNDsVR7AWUv3GLXCm6nqT787RW+Zpgc9frlclEXhdHWKfmyaZUAVO0njyONut81BbsmC4qd/GoI/eT/EcT+zAGgeLhaA4je9fdqhya3ASLYqkMPUjT+zc="), _sql;

        public void LimparCampos()
        {
            txt_Registro.Clear();
            cb_Disciplina.SelectedIndex = -1;
            txt_Autor.Clear();
            cb_Ensino.SelectedIndex = -1;
            cb_Volume.SelectedIndex = -1;
            dtDataRegistro.Text = DateTime.Now.ToShortDateString();
            btn_Salvar.Text = "Salvar - F1";
            btn_Salvar.Image = Properties.Resources.Zerode_Plump_Drive_Floppy_blue;
        }

        private void txt_Registro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Salvar_Click(sender, e);
            }
        }

        private void btn_Salvar_Click(object sender, EventArgs e)
        {
            switch (btn_Salvar.Text)
            {
                case "Salvar - F1":
                    validarCampos();
                    try
                    {
                        if (valido)
                        {
                            if (!SalvarLivroDidatico())
                                return;
                            btn_Salvar.Text = "Incluir - F1";
                            btn_Salvar.Image = Properties.Resources.Actions_list_add_icon;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "Incluir - F1":
                    DialogResult dr = MessageBox.Show("Incluir outro livro?", "Biblioteca Fácil", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (dr == DialogResult.No)
                    {
                        btn_Salvar.Text = "Salvar - F1";
                        btn_Salvar.Image = Properties.Resources.Zerode_Plump_Drive_Floppy_blue;
                        txt_Registro.Focus();
                        txt_Registro.Clear();
                        return;
                    }

                    recarregarFormatoPadrao();                    
                    break;
            }

        }

        private void recarregarFormatoPadrao()
        {
            LimparCampos();
            btn_Salvar.Text = "Salvar - F1";
            btn_Salvar.Image = Properties.Resources.Zerode_Plump_Drive_Floppy_blue;
            txt_Registro.Focus();            
        }

        private bool SalvarLivroDidatico()
        {
            didatico.registro = int.Parse(txt_Registro.Text);
            didatico.disciplina = cb_Disciplina.Text;
            didatico.autor = txt_Autor.Text;
            didatico.ensino = cb_Ensino.Text.ToUpper();
            didatico.volume = cb_Volume.Text.ToUpper();
            didatico.dataRegistro = dtDataRegistro.Text;
            if (didatico.Cadastrar() == true)
            {
                try
                {
                    didatico.Cadastrar();
                    MessageBox.Show("Livro cadastrado com sucesso!", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_Registro.Focus();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("O número do registro já existe!", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txt_Registro.Clear();
                txt_Registro.Focus();
                return false;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            validarCampos();
            if (valido)
            {
                didatico.registro = int.Parse(txt_Registro.Text);
                didatico.disciplina = cb_Disciplina.Text;
                didatico.autor = txt_Autor.Text;
                didatico.ensino = cb_Ensino.Text.ToUpper();
                didatico.volume = cb_Volume.Text.ToUpper();
                didatico.dataRegistro = dtDataRegistro.Text;
                if (didatico.Atualizar() == true)
                {
                    try
                    {
                        didatico.Atualizar();
                        MessageBox.Show("Livro atualizado com sucesso!", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txt_Registro.Focus();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Não encontramos livros com este registro! Tente outra opção...", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                recarregarFormatoPadrao();
            }
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (txt_Registro.Text != "")
            {
                didatico.registro = int.Parse(txt_Registro.Text);
                if (didatico.Buscar() == true)
                {
                    if (buscarEmprestimoLivroDidatico() == 1)
                    {
                        SqlConnection conexao = new SqlConnection(stringConn);
                        _sql = "Delete from Emprestimo_Livro_Didatico where N_Registro = " + txt_Registro.Text + " and Data_Solicitacao <>'' and Data_Entrega <>''";
                        SqlCommand comando = new SqlCommand(_sql, conexao);
                        comando.CommandText = _sql;
                        try
                        {
                            conexao.Open();
                            comando.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        conexao.Close();

                        Excluir();

                    }
                    else if (buscarEmprestimoLivroDidatico() == 2)
                    {
                        MessageBox.Show("É necessário quitar todos os livros emprestados para que seja feita a exclusão do livro na base de dados!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    else if (buscarEmprestimoLivroDidatico() == 0)
                    {
                        Excluir();
                    }
                }
                else
                    MessageBox.Show("Não há livros com este registro! Tente outra opção...", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                error_Provider.Clear();
                error_Provider.SetError(txt_Registro, "Campo inválido!");
                txt_Registro.Focus();
                MessageBox.Show("Campo vazio! Informe o registro!", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
        }
        private void Excluir()
        {

            didatico.registro = int.Parse(txt_Registro.Text);
            try
            {
                if (MessageBox.Show("Tem certeza que deseja excluir este registro?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    didatico.Deletar();
                    MessageBox.Show("Dados excluido com sucesso", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    recarregarFormatoPadrao();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int buscarEmprestimoLivroDidatico()
        {
            try
            {
                SqlConnection conexao = new SqlConnection(stringConn);
                string _sql = "Select * from emprestimo_livro_Didatico where N_registro = " + txt_Registro.Text; SqlDataAdapter adapter = new SqlDataAdapter(_sql, conexao);
                adapter.SelectCommand.CommandText = _sql;
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {

                    string Solicitacao = table.Rows[0]["Data_Solicitacao"].ToString();
                    string Entrega = table.Rows[0]["Data_Entrega"].ToString();

                    if ((Solicitacao != "") && (Entrega != ""))
                    {
                        return 1;
                    }
                    else if ((Solicitacao != "") && (Entrega == ""))
                    {
                        return 2;
                    }
                    return 3;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }

        bool valido = false;
        int n_registro;

        private void validarCampos()
        {
            if (txt_Registro.Text != "")
                n_registro = int.Parse(txt_Registro.Text);

            error_Provider.Clear();
            if (txt_Registro.Text.Trim().Equals(""))
            {
                error_Provider.SetError(txt_Registro, "Campo Obrigatório!");
                txt_Registro.Focus();
                valido = false;
                return;
            }
            else if (cb_Disciplina.Text.Trim().Equals(""))
            {
                error_Provider.SetError(cb_Disciplina, "Campo Obrigatório!");
                cb_Disciplina.Focus();
                valido = false;
                return;
            }
            else if (cb_Ensino.Text.Trim().Equals(""))
            {
                error_Provider.SetError(cb_Ensino, "Campo Obrigatório!");
                cb_Ensino.Focus();
                valido = false;
                return;
            }
            else if (cb_Volume.Text.Trim().Equals(""))
            {
                error_Provider.SetError(cb_Volume, "Campo Obrigatório!");
                cb_Volume.Focus();
                valido = false;
                return;
            }
            else if (n_registro == 0)
            {
                MessageBox.Show("Valor do Registro Inválido!", "Biblioteca Fácil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valido = false;
                return;
            }
            else
                valido = true;
        }

        private void btn_Sair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txt_Conteudo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Salvar_Click(sender, e);
            }
        }

        private void txt_Autor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Salvar_Click(sender, e);
            }
        }

        private void cb_Ensino_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Salvar_Click(sender, e);
            }
        }

        private void cb_Volume_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Salvar_Click(sender, e);
            }
        }

        private void txt_Registro_KeyPress(object sender, KeyPressEventArgs e)
        {
            //aceita só números
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        private void txt_Autor_KeyPress(object sender, KeyPressEventArgs e)
        {
            //aceita só letras
            if (Char.IsDigit(e.KeyChar) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        private void txt_Registro_TextChanged(object sender, EventArgs e)
        {
            error_Provider.Clear();
        }

        private void txt_Disciplina_TextChanged(object sender, EventArgs e)
        {
            error_Provider.Clear();
        }

        private void cb_Ensino_SelectedIndexChanged(object sender, EventArgs e)
        {
            error_Provider.Clear();
        }

        private void cb_Volume_SelectedIndexChanged(object sender, EventArgs e)
        {
            error_Provider.Clear();
        }

        private void cb_Disciplina_SelectedIndexChanged(object sender, EventArgs e)
        {
            error_Provider.Clear();
        }

        private void FrmLivroDidatico_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btn_Salvar_Click(sender, e);
            }
            if (e.KeyCode == Keys.F2)
            {
                btnPesquisar_Click(sender, e);
            }
            if (e.KeyCode == Keys.F3)
            {
                btnEditar_Click(sender, e);
            }
            if (e.KeyCode == Keys.F4)
            {
                BtnExcluir_Click(sender, e);
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            FrmBuscarLivroDidatico buscarLivroDidatico = new FrmBuscarLivroDidatico();
            buscarLivroDidatico.ShowDialog();
            if (buscarLivroDidatico.registro > 0)
            {
                txt_Registro.Text = buscarLivroDidatico.registro.ToString();
                cb_Disciplina.Text = buscarLivroDidatico.disciplina;
                cb_Ensino.Text = buscarLivroDidatico.ensino;
                cb_Volume.Text = buscarLivroDidatico.volume;
                txt_Autor.Text = buscarLivroDidatico.autor;
                if (!string.IsNullOrEmpty(buscarLivroDidatico.DataRegistro))
                    dtDataRegistro.Text = buscarLivroDidatico.DataRegistro;
                else
                    dtDataRegistro.Text = DateTime.Now.ToShortDateString();
            }
        }
    }
}
